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
        // Check if pet belongs to current user
        var currentUserId = _userContext.GetCurrentUserId();
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == dto.PetId && p.OwnerId == currentUserId);
        if (pet == null)
        {
            throw new UnauthorizedAccessException("Pet not found or does not belong to user");
        }

        // Check vet availability only if veterinarian is specified
        if (dto.VeterinarianId.HasValue)
        {
            var overlapping = await _context.Appointments
                .Where(a => a.VeterinarianId == dto.VeterinarianId &&
                           a.Status != AppointmentStatus.Cancelled &&
                           ((dto.StartAt < a.EndAt && a.StartAt < dto.EndAt)))
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
                VeterinarianId = dto.VeterinarianId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Status = AppointmentStatus.Scheduled
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
            // Vets see their appointments
            query = query.Where(a => a.VeterinarianId == userId);
        }
        else
        {
            // Owners see their pets' appointments
            query = query.Where(a => a.Pet.OwnerId == userId);
        }

        // Additional filters
        if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out var parsedDate))
        {
            var filterDate = parsedDate.Date;
            query = query.Where(a => a.StartAt.Date == filterDate);
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
}
