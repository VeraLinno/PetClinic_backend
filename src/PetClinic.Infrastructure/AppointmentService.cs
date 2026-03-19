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
        var isVet = roles.Contains("Vet");

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
        var query = _context.Appointments
            .Include(a => a.Pet)
            .Include(a => a.Veterinarian)
            .AsQueryable();

        if (roles.Contains("Vet"))
        {
            // Vets see their own appointments and the unassigned queue.
            query = query.Where(a => a.VeterinarianId == userId || a.VeterinarianId == null);
        }
        else
        {
            // Owners see their pets' appointments
            query = query.Where(a => a.Pet.OwnerId == userId);
        }

        // Additional filters
        if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out var parsedDate))
        {
            var filterDate = NormalizeToUtc(parsedDate.Date);
            query = query.Where(a => a.StartAt.Date == filterDate.Date);
        }
        if (ownerId.HasValue)
        {
            query = query.Where(a => a.Pet.OwnerId == ownerId.Value);
        }
        if (vetId.HasValue)
        {
            query = query.Where(a => a.VeterinarianId == vetId.Value);
        }

        return await query.ToListAsync();
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
        if (!roles.Contains("Vet"))
        {
            throw new UnauthorizedAccessException("Only veterinarians can confirm appointments");
        }

        if (appointment.VeterinarianId.HasValue && appointment.VeterinarianId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only confirm your own appointments");
        }

        if (!appointment.VeterinarianId.HasValue)
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

        if (roles.Contains("Vet"))
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
