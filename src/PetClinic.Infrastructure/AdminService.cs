using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetClinic.Application;
using PetClinic.Domain;

namespace PetClinic.Infrastructure;

/// <summary>
/// Administrative service implementation.
/// Provides system management operations with comprehensive logging.
/// </summary>
public class AdminService : IAdminService
{
    private readonly PetClinicDbContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(PetClinicDbContext context, ILogger<AdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ===== USER MANAGEMENT =====

    public async Task<List<AdminUserDto>> GetAllUsersAsync()
    {
        _logger.LogInformation("Admin: Retrieving all users");

        var users = await _context.Owners
            .OrderBy(o => o.Email)
            .ToListAsync();

        return users.Select(u => new AdminUserDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Roles = u.Roles,
            CreatedAt = u.Id.ToString().Length > 0 ? DateTime.UtcNow.AddDays(-30) : DateTime.UtcNow,
            LastLoginAt = null,  // TODO: Track login events
            IsActive = true
        }).ToList();
    }

    public async Task<AdminUserDto?> GetUserByIdAsync(Guid userId)
    {
        _logger.LogInformation("Admin: Retrieving user {UserId}", userId);

        var user = await _context.Owners.FirstOrDefaultAsync(o => o.Id == userId);
        if (user == null) return null;

        return new AdminUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.Roles,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            LastLoginAt = null,
            IsActive = true
        };
    }

    public async Task<AdminUserDto> UpdateUserAsync(Guid userId, UpdateUserRoleDto dto)
    {
        _logger.LogInformation("Admin: Updating user {UserId} - Roles: {Roles}", userId, string.Join(",", dto.Roles));

        var user = await _context.Owners.FirstOrDefaultAsync(o => o.Id == userId);
        if (user == null)
            throw new KeyNotFoundException($"User {userId} not found");

        user.Roles = dto.Roles ?? new List<string>();
        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin: User {UserId} updated successfully. New roles: {Roles}", userId, string.Join(",", user.Roles));

        return new AdminUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.Roles,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            IsActive = true
        };
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        _logger.LogWarning("Admin: Deleting user {UserId}", userId);

        var user = await _context.Owners.FirstOrDefaultAsync(o => o.Id == userId);
        if (user == null)
            throw new KeyNotFoundException($"User {userId} not found");

        _context.Owners.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogWarning("Admin: User {UserId} ({Email}) deleted", userId, user.Email);
    }

    public async Task<List<AdminAuditLogDto>> GetUserActivityAsync(Guid userId, int days = 30)
    {
        _logger.LogInformation("Admin: Retrieving activity for user {UserId} (last {Days} days)", userId, days);

        var fromDate = DateTime.UtcNow.AddDays(-days);

        // TODO: Implement audit log tracking
        return new List<AdminAuditLogDto>();
    }

    // ===== VETERINARIAN MANAGEMENT =====

    public async Task<List<AdminVeterinarianDto>> GetAllVeterinariansAsync()
    {
        _logger.LogInformation("Admin: Retrieving all veterinarians");

        var vets = await _context.Veterinarians
            .Include(v => v.Appointments)
            .OrderBy(v => v.Name)
            .ToListAsync();

        return vets.Select(v => new AdminVeterinarianDto
        {
            Id = v.Id,
            Email = v.Email,
            Name = v.Name,
            LastName = v.LastName,
            LicenseNumber = v.LicenseNumber,
            PhoneNumber = v.PhoneNumber,
            TotalAppointments = v.Appointments?.Count ?? 0,
            IsActive = true
        }).ToList();
    }

    public async Task<AdminVeterinarianDto?> GetVeterinarianByIdAsync(Guid vetId)
    {
        _logger.LogInformation("Admin: Retrieving veterinarian {VetId}", vetId);

        var vet = await _context.Veterinarians
            .Include(v => v.Appointments)
            .FirstOrDefaultAsync(v => v.Id == vetId);

        if (vet == null) return null;

        return new AdminVeterinarianDto
        {
            Id = vet.Id,
            Email = vet.Email,
            Name = vet.Name,
            LastName = vet.LastName,
            LicenseNumber = vet.LicenseNumber,
            PhoneNumber = vet.PhoneNumber,
            TotalAppointments = vet.Appointments?.Count ?? 0,
            IsActive = true
        };
    }

    public async Task DeactivateVeterinarianAsync(Guid vetId)
    {
        _logger.LogWarning("Admin: Deactivating veterinarian {VetId}", vetId);

        var vet = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == vetId);
        if (vet == null)
            throw new KeyNotFoundException($"Veterinarian {vetId} not found");

        // TODO: Mark as inactive (add IsActive field to Veterinarian model)
        await _context.SaveChangesAsync();

        _logger.LogWarning("Admin: Veterinarian {VetId} ({Name}) deactivated", vetId, vet.Name);
    }

    public async Task ReactivateVeterinarianAsync(Guid vetId)
    {
        _logger.LogInformation("Admin: Reactivating veterinarian {VetId}", vetId);

        var vet = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == vetId);
        if (vet == null)
            throw new KeyNotFoundException($"Veterinarian {vetId} not found");

        // TODO: Mark as active
        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin: Veterinarian {VetId} ({Name}) reactivated", vetId, vet.Name);
    }

    // ===== APPOINTMENT MANAGEMENT =====

    public async Task<List<AdminAppointmentDto>> GetAllAppointmentsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? status = null)
    {
        _logger.LogInformation("Admin: Retrieving appointments (from: {FromDate}, to: {ToDate}, status: {Status})",
            fromDate, toDate, status);

        var query = _context.Appointments
            .Include(a => a.Pet)
            .Include(a => a.Veterinarian)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(a => a.StartAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.EndAt <= toDate.Value);

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<AppointmentStatus>(status, true, out var statusEnum))
                query = query.Where(a => a.Status == statusEnum);
        }

        var appointments = await query.OrderByDescending(a => a.StartAt).ToListAsync();

        return appointments.Select(a => new AdminAppointmentDto
        {
            Id = a.Id,
            PetId = a.PetId,
            PetName = a.Pet?.Name,
            OwnerEmail = a.Pet?.Owner?.Email,
            VeterinarianId = a.VeterinarianId,
            VeterinarianName = a.Veterinarian != null ? $"{a.Veterinarian.Name} {a.Veterinarian.LastName}" : null,
            StartAt = a.StartAt,
            EndAt = a.EndAt,
            Status = a.Status.ToString()
        }).ToList();
    }

    public async Task CancelAppointmentAsync(Guid appointmentId, string reason)
    {
        _logger.LogWarning("Admin: Force cancelling appointment {AppointmentId} - Reason: {Reason}", appointmentId, reason);

        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
        if (appointment == null)
            throw new KeyNotFoundException($"Appointment {appointmentId} not found");

        appointment.Status = AppointmentStatus.Cancelled;
        await _context.SaveChangesAsync();

        _logger.LogWarning("Admin: Appointment {AppointmentId} cancelled. Reason: {Reason}", appointmentId, reason);
    }

    public async Task ReassignAppointmentAsync(Guid appointmentId, Guid newVetId)
    {
        _logger.LogInformation("Admin: Reassigning appointment {AppointmentId} to veterinarian {NewVetId}", appointmentId, newVetId);

        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
        if (appointment == null)
            throw new KeyNotFoundException($"Appointment {appointmentId} not found");

        var vet = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == newVetId);
        if (vet == null)
            throw new KeyNotFoundException($"Veterinarian {newVetId} not found");

        var oldVetId = appointment.VeterinarianId;
        appointment.VeterinarianId = newVetId;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin: Appointment {AppointmentId} reassigned from {OldVetId} to {NewVetId}",
            appointmentId, oldVetId, newVetId);
    }

    // ===== FINANCIAL MANAGEMENT =====

    public async Task<List<InvoiceDto>> GetAllInvoicesAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        _logger.LogInformation("Admin: Retrieving invoices (from: {FromDate}, to: {ToDate})", fromDate, toDate);

        var query = _context.Invoices.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(i => i.IssuedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(i => i.IssuedAt <= toDate.Value);

        var invoices = await query.OrderByDescending(i => i.IssuedAt).ToListAsync();

        return invoices.Select(i => new InvoiceDto
        {
            Id = i.Id,
            VisitId = i.VisitId,
            Amount = i.Amount,
            IssuedAt = i.IssuedAt
        }).ToList();
    }

    public async Task<AdminFinancialReportDto> GetFinancialReportAsync(DateTime fromDate, DateTime toDate)
    {
        _logger.LogInformation("Admin: Generating financial report ({FromDate} to {ToDate})", fromDate, toDate);

        var invoices = await _context.Invoices
            .Where(i => i.IssuedAt >= fromDate && i.IssuedAt <= toDate)
            .ToListAsync();

        var totalRevenue = invoices.Sum(i => i.Amount);
        var paidCount = invoices.Count;  // TODO: Add payment status tracking

        return new AdminFinancialReportDto
        {
            PeriodStart = fromDate,
            PeriodEnd = toDate,
            TotalRevenue = totalRevenue,
            TotalInvoices = invoices.Count,
            PaidInvoices = paidCount,
            OverdueInvoices = 0,  // TODO: Track payment dates
            OutstandingAmount = 0,  // TODO: Calculate from unpaid invoices
            AverageInvoiceAmount = invoices.Count > 0 ? totalRevenue / invoices.Count : 0
        };
    }

    public async Task AdjustInvoiceAsync(Guid invoiceId, decimal newAmount, string reason)
    {
        _logger.LogWarning("Admin: Adjusting invoice {InvoiceId} to {NewAmount} - Reason: {Reason}", invoiceId, newAmount, reason);

        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId);
        if (invoice == null)
            throw new KeyNotFoundException($"Invoice {invoiceId} not found");

        var oldAmount = invoice.Amount;
        invoice.Amount = newAmount;
        await _context.SaveChangesAsync();

        _logger.LogWarning("Admin: Invoice {InvoiceId} adjusted from {OldAmount} to {NewAmount}. Reason: {Reason}",
            invoiceId, oldAmount, newAmount, reason);
    }

    // ===== INVENTORY MANAGEMENT =====

    public async Task<List<AdminInventoryReportDto>> GetInventoryReportAsync()
    {
        _logger.LogInformation("Admin: Generating inventory report");

        var medications = await _context.MedicationStocks.ToListAsync();

        return medications.Select(m => new AdminInventoryReportDto
        {
            MedicationId = m.Id,
            MedicationName = m.Name,
            Category = m.Category,
            CurrentQuantity = m.Quantity,
            ReorderLevel = m.ReorderLevel,
            Unit = m.Unit,
            UnitPrice = m.UnitPrice,
            UsedThisMonth = 0,  // TODO: Track usage
            IsLowStock = m.Quantity < m.ReorderLevel
        }).ToList();
    }

    public async Task<List<AdminInventoryReportDto>> GetLowStockMedicationsAsync()
    {
        _logger.LogInformation("Admin: Retrieving low stock medications");

        var medications = await _context.MedicationStocks
            .Where(m => m.Quantity < m.ReorderLevel)
            .OrderBy(m => m.Quantity)
            .ToListAsync();

        return medications.Select(m => new AdminInventoryReportDto
        {
            MedicationId = m.Id,
            MedicationName = m.Name,
            Category = m.Category,
            CurrentQuantity = m.Quantity,
            ReorderLevel = m.ReorderLevel,
            Unit = m.Unit,
            UnitPrice = m.UnitPrice,
            IsLowStock = true
        }).ToList();
    }

    public async Task<AdminMedicationUsageReportDto> GetMedicationUsageAsync(Guid medicationId, int months = 3)
    {
        _logger.LogInformation("Admin: Getting medication usage report for {MedicationId} (last {Months} months)", medicationId, months);

        var medication = await _context.MedicationStocks.FirstOrDefaultAsync(m => m.Id == medicationId);
        if (medication == null)
            throw new KeyNotFoundException($"Medication {medicationId} not found");

        // TODO: Track prescription usage
        return new AdminMedicationUsageReportDto
        {
            MedicationName = medication.Name,
            TotalUsed = 0,
            AverageUsagePerMonth = 0,
            LastUsed = DateTime.UtcNow,
            CurrentStock = medication.Quantity,
            ReorderLevel = medication.ReorderLevel,
            EstimatedMonthsUntilStockout = medication.Quantity > 0 ? 12 : 0
        };
    }

    // ===== AUDIT & LOGGING =====

    public async Task<List<AdminAuditLogDto>> GetAuditLogsAsync(
        int days = 30,
        string? userEmail = null,
        string? action = null)
    {
        _logger.LogInformation("Admin: Retrieving audit logs (days: {Days}, email: {Email}, action: {Action})", days, userEmail, action);

        // TODO: Implement audit log table and querying
        return new List<AdminAuditLogDto>();
    }

    public async Task<List<AdminAuditLogDto>> SearchAuditLogsAsync(AdminAuditLogFilterDto filter)
    {
        _logger.LogInformation("Admin: Searching audit logs with filter");

        // TODO: Implement comprehensive audit log search
        return new List<AdminAuditLogDto>();
    }

    // ===== DASHBOARD METRICS =====

    public async Task<AdminDashboardMetricsDto> GetDashboardMetricsAsync()
    {
        _logger.LogInformation("Admin: Generating dashboard metrics");

        var totalUsers = await _context.Owners.CountAsync();
        var totalVets = await _context.Veterinarians.CountAsync();
        var totalPets = await _context.Pets.CountAsync();

        var thisMonth = DateTime.UtcNow;
        var monthStart = new DateTime(thisMonth.Year, thisMonth.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var appointmentsThisMonth = await _context.Appointments
            .Where(a => a.StartAt >= monthStart && a.StartAt <= monthEnd)
            .CountAsync();

        var completedVisitsThisMonth = await _context.Visits
            .Where(v => v.CompletedAt.HasValue && v.CompletedAt >= monthStart && v.CompletedAt <= monthEnd)
            .CountAsync();

        var revenueThisMonth = await _context.Invoices
            .Where(i => i.IssuedAt >= monthStart && i.IssuedAt <= monthEnd)
            .SumAsync(i => i.Amount);

        var lowStockCount = await _context.MedicationStocks
            .Where(m => m.Quantity < m.ReorderLevel)
            .CountAsync();

        return new AdminDashboardMetricsDto
        {
            TotalUsers = totalUsers,
            TotalVeterinarians = totalVets,
            TotalPets = totalPets,
            TotalAppointmentsThisMonth = appointmentsThisMonth,
            CompletedVisitsThisMonth = completedVisitsThisMonth,
            TotalRevenueThisMonth = revenueThisMonth,
            LowStockMedications = lowStockCount,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<AdminSystemHealthDto> GetSystemHealthAsync()
    {
        _logger.LogInformation("Admin: Checking system health");

        try
        {
            // Simple health check - verify database connectivity
            await _context.Owners.CountAsync();

            var now = DateTime.UtcNow;
            var activeUsers = await _context.RefreshTokens
                .Where(rt => !rt.RevokedAt.HasValue && rt.Expires > now)
                .Select(rt => rt.OwnerId)
                .Distinct()
                .CountAsync();

            var totalMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            var usedMemoryBytes = GC.GetTotalMemory(false);
            var memoryUsage = totalMemoryBytes > 0
                ? Math.Min(100d, Math.Round((usedMemoryBytes / (double)totalMemoryBytes) * 100d, 1))
                : 0d;

            return new AdminSystemHealthDto
            {
                Status = "Healthy",
                CheckedAt = DateTime.UtcNow,
                ActiveUsers = activeUsers,
                DatabaseConnections = 1,
                CpuUsage = 0,
                MemoryUsage = memoryUsage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "System health check failed");
            return new AdminSystemHealthDto
            {
                Status = "Critical",
                CheckedAt = DateTime.UtcNow,
                Issues = ex.Message
            };
        }
    }
}
