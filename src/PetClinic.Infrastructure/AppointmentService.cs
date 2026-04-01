using Microsoft.EntityFrameworkCore;
using PetClinic.Application;
using PetClinic.Domain;

namespace PetClinic.Infrastructure;

public class AppointmentService : IAppointmentService
{
    private readonly PetClinicDbContext _context;
    private readonly IUserContextService _userContext;

    public AppointmentService(PetClinicDbContext context, IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<Appointment> CreateAsync(CreateAppointmentDto dto)
    {
        var normalizedStartAt = NormalizeToUtc(dto.StartAt);
        var normalizedEndAt = NormalizeToUtc(dto.EndAt);

        if (normalizedEndAt <= normalizedStartAt)
        {
            throw new InvalidOperationException("Appointment end time must be after start time");
        }

        if (normalizedStartAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Cannot create appointment in the past");
        }

        var currentUserId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();
        var isVet = roles.Contains("Vet") || roles.Contains("Admin");

        // Owners can only book for their own pets; vets can book for any pet.
        var pet = isVet
            ? await _context.Pets.FirstOrDefaultAsync(p => p.Id == dto.PetId)
            : await _context.Pets.FirstOrDefaultAsync(p => p.Id == dto.PetId && p.OwnerId == currentUserId);

        if (pet == null)
        {
            throw new UnauthorizedAccessException("Pet not found or does not belong to user");
        }

        Guid? assignedVeterinarianId = dto.VeterinarianId;

        // Vet-created bookings default to the current vet when no explicit assignment is provided.
        if (isVet && !assignedVeterinarianId.HasValue)
        {
            assignedVeterinarianId = currentUserId;
        }

        if (!assignedVeterinarianId.HasValue)
        {
            throw new InvalidOperationException("Veterinarian selection is required");
        }

        if (assignedVeterinarianId.HasValue)
        {
            var veterinarianExists = await _context.Veterinarians.AnyAsync(v => v.Id == assignedVeterinarianId.Value);
            if (!veterinarianExists)
            {
                throw new InvalidOperationException("Selected veterinarian does not exist");
            }
        }

        // Check vet availability only if veterinarian is specified
        if (assignedVeterinarianId.HasValue)
        {
            var overlapping = await _context.Appointments
                .Where(a => a.VeterinarianId == assignedVeterinarianId &&
                           a.Status != AppointmentStatus.Cancelled &&
                           ((normalizedStartAt < a.EndAt && a.StartAt < normalizedEndAt)))
                .AnyAsync();

            if (overlapping)
            {
                throw new InvalidOperationException("Veterinarian is not available at this time");
            }
        }

        // Use transaction for concurrency
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var appointment = new Appointment
            {
                PetId = dto.PetId,
                VeterinarianId = assignedVeterinarianId,
                StartAt = normalizedStartAt,
                EndAt = normalizedEndAt,
                Status = isVet ? AppointmentStatus.Scheduled : AppointmentStatus.Pending
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return appointment;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Appointment>> GetUserAppointmentsAsync(Guid userId, List<string> roles, string? date = null, Guid? ownerId = null, Guid? vetId = null)
    {
        var query = from appointment in _context.Appointments.AsNoTracking()
                    join pet in _context.Pets.AsNoTracking() on appointment.PetId equals pet.Id
                    join veterinarian in _context.Veterinarians.AsNoTracking() on appointment.VeterinarianId equals veterinarian.Id into veterinarianJoin
                    from veterinarian in veterinarianJoin.DefaultIfEmpty()
                    select new
                    {
                        Appointment = appointment,
                        PetId = pet.Id,
                        PetName = pet.Name,
                        PetOwnerId = pet.OwnerId,
                        VeterinarianId = appointment.VeterinarianId,
                        VeterinarianName = veterinarian != null ? veterinarian.Name : null,
                        VeterinarianLastName = veterinarian != null ? veterinarian.LastName : null,
                        VeterinarianEmail = veterinarian != null ? veterinarian.Email : null
                    };

        if (roles.Contains("Vet"))
        {
            // Vets see their own appointments and the unassigned queue.
            query = query.Where(a => a.VeterinarianId == userId || a.VeterinarianId == null);
        }
        else if (roles.Contains("Admin"))
        {
            // Admins can review all appointments while using admin view modes.
        }
        else
        {
            // Owners see their pets' appointments
            query = query.Where(a => a.PetOwnerId == userId);
        }

        // Additional filters
        if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out var parsedDate))
        {
            var filterDate = NormalizeToUtc(parsedDate.Date);
            query = query.Where(a => a.Appointment.StartAt.Date == filterDate.Date);
        }
        if (ownerId.HasValue)
        {
            query = query.Where(a => a.PetOwnerId == ownerId.Value);
        }
        if (vetId.HasValue)
        {
            query = query.Where(a => a.VeterinarianId == vetId.Value);
        }

        var rows = await query
            .OrderBy(a => a.Appointment.StartAt)
            .ToListAsync();

        return rows.Select(row => new Appointment
        {
            Id = row.Appointment.Id,
            PetId = row.Appointment.PetId,
            VeterinarianId = row.Appointment.VeterinarianId,
            StartAt = row.Appointment.StartAt,
            EndAt = row.Appointment.EndAt,
            Status = row.Appointment.Status,
            Visit = row.Appointment.Visit,
            Pet = new Pet
            {
                Id = row.PetId,
                Name = row.PetName,
                OwnerId = row.PetOwnerId
            },
            Veterinarian = row.VeterinarianId.HasValue
                ? new Veterinarian
                {
                    Id = row.VeterinarianId.Value,
                    Name = row.VeterinarianName,
                    LastName = row.VeterinarianLastName,
                    Email = row.VeterinarianEmail
                }
                : null
        }).ToList();
    }

    public async Task ConfirmAsync(Guid appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Pet)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);
        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found");
        }

        var currentUserId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();
        var isVet = roles.Contains("Vet");
        var isAdmin = roles.Contains("Admin");
        if (!isVet && !isAdmin)
        {
            throw new UnauthorizedAccessException("Only veterinarians can confirm appointments");
        }

        if (isVet && appointment.VeterinarianId.HasValue && appointment.VeterinarianId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only confirm your own appointments");
        }

        if (isVet && !appointment.VeterinarianId.HasValue)
        {
            appointment.VeterinarianId = currentUserId;
        }

        if (appointment.Status == AppointmentStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot confirm a cancelled appointment");
        }

        if (appointment.Status == AppointmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot confirm a completed appointment");
        }

        appointment.Status = AppointmentStatus.Scheduled;
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task CancelAsync(Guid appointmentId)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Pet)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);
        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found");
        }

        // Check authorization: owner can cancel their own pet's appointment, vet can cancel their appointments
        var currentUserId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();

        if (roles.Contains("Admin"))
        {
            // Admin can cancel any appointment.
        }
        else if (roles.Contains("Vet"))
        {
            if (appointment.VeterinarianId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only cancel your own appointments");
            }
        }
        else
        {
            if (appointment.Pet.OwnerId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only cancel appointments for your own pets");
            }
        }

        if (appointment.Status == AppointmentStatus.Cancelled)
        {
            throw new InvalidOperationException("Appointment is already cancelled");
        }

        if (appointment.Status == AppointmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed appointment");
        }

        appointment.Status = AppointmentStatus.Cancelled;
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    private static DateTime NormalizeToUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        if (value.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        return value.ToUniversalTime();
    }
}
