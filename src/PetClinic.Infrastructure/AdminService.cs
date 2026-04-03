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
            .AsNoTracking()
            .Select(o => new
            {
                o.Id,
                o.Email,
                o.FirstName,
                o.LastName,
                o.Roles,
                o.LastLoginAt
            })
            .OrderBy(o => o.Email)
            .ToListAsync();

        return users.Select(u => new AdminUserDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Roles = u.Roles,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            LastLoginAt = u.LastLoginAt,
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
            LastLoginAt = user.LastLoginAt,
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

            var logs = await _context.AdminAuditEvents
                .Where(a => a.PerformedByUserId == userId && a.OccurredAtUtc >= fromDate)
                .OrderByDescending(a => a.OccurredAtUtc)
                .ToListAsync();

            return logs.Select(l => new AdminAuditLogDto
            {
                Id = l.Id,
                UserId = l.PerformedByUserId,
                UserEmail = l.PerformedByEmail,
                Action = l.Action,
                Endpoint = $"{l.TargetType}/{l.TargetId}",
                StatusCode = 200,
                Timestamp = l.OccurredAtUtc,
                Details = l.MetadataJson
            }).ToList();
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
              IsActive = v.IsActive
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
              IsActive = vet.IsActive
        };
    }

    public async Task DeactivateVeterinarianAsync(Guid vetId)
    {
        _logger.LogWarning("Admin: Deactivating veterinarian {VetId}", vetId);

        var vet = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == vetId);
        if (vet == null)
            throw new KeyNotFoundException($"Veterinarian {vetId} not found");

            vet.IsActive = false;
        await _context.SaveChangesAsync();

        _logger.LogWarning("Admin: Veterinarian {VetId} ({Name}) deactivated", vetId, vet.Name);
    }

    public async Task ReactivateVeterinarianAsync(Guid vetId)
    {
        _logger.LogInformation("Admin: Reactivating veterinarian {VetId}", vetId);

        var vet = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == vetId);
        if (vet == null)
            throw new KeyNotFoundException($"Veterinarian {vetId} not found");

            vet.IsActive = true;
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

        var query = from appointment in _context.Appointments.AsNoTracking()
                    join pet in _context.Pets.AsNoTracking() on appointment.PetId equals pet.Id into petJoin
                    from pet in petJoin.DefaultIfEmpty()
                    join owner in _context.Owners.AsNoTracking() on pet.OwnerId equals owner.Id into ownerJoin
                    from owner in ownerJoin.DefaultIfEmpty()
                    join vet in _context.Veterinarians.AsNoTracking() on appointment.VeterinarianId equals vet.Id into vetJoin
                    from vet in vetJoin.DefaultIfEmpty()
                    select new
                    {
                        appointment.Id,
                        appointment.PetId,
                        PetName = pet != null ? pet.Name : null,
                        OwnerEmail = owner != null ? owner.Email : null,
                        appointment.VeterinarianId,
                        VetName = vet != null ? vet.Name : null,
                        VetLastName = vet != null ? vet.LastName : null,
                        appointment.StartAt,
                        appointment.EndAt,
                        appointment.Status
                    };

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
            PetName = a.PetName,
            OwnerEmail = a.OwnerEmail,
            VeterinarianId = a.VeterinarianId,
            VeterinarianName = a.VeterinarianId.HasValue ? $"{a.VetName} {a.VetLastName}".Trim() : null,
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
            IssuedAt = i.IssuedAt,
            Status = i.Status.ToString(),
            PaidAt = i.PaidAt,
            DueDate = i.DueDate
        }).ToList();
    }

    public async Task<AdminFinancialReportDto> GetFinancialReportAsync(DateTime fromDate, DateTime toDate)
    {
        _logger.LogInformation("Admin: Generating financial report ({FromDate} to {ToDate})", fromDate, toDate);

        var invoices = await _context.Invoices
            .Where(i => i.IssuedAt >= fromDate && i.IssuedAt <= toDate)
            .ToListAsync();

        var totalRevenue = invoices.Sum(i => i.Amount);

        return new AdminFinancialReportDto
        {
            PeriodStart = fromDate,
            PeriodEnd = toDate,
            TotalRevenue = totalRevenue,
            TotalInvoices = invoices.Count,
                PaidInvoices = invoices.Count(i => i.Status == Invoice.PaymentStatus.Paid),
                OverdueInvoices = invoices.Count(i => i.Status == Invoice.PaymentStatus.Overdue && (!i.DueDate.HasValue || i.DueDate.Value < DateTime.UtcNow)),
                OutstandingAmount = invoices.Where(i => i.Status == Invoice.PaymentStatus.Unpaid).Sum(i => i.Amount),
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
        var usage = await _context.Prescriptions
            .GroupBy(p => p.Medication)
            .Select(g => new { Medication = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.Medication, x => x.Quantity);

        return medications.Select(m => new AdminInventoryReportDto
        {
            MedicationId = m.Id,
            MedicationName = m.Name,
            Category = m.Category,
            CurrentQuantity = m.Quantity,
            ReorderLevel = m.ReorderLevel,
            Unit = m.Unit,
            UnitPrice = m.UnitPrice,
            UsedThisMonth = usage.TryGetValue(m.Name, out var usedQty) ? usedQty : 0,
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

        var safeMonths = Math.Max(1, months);
        var usageWindowStart = DateTime.UtcNow.AddMonths(-safeMonths);

        var usageQuery = _context.Prescriptions
            .Where(p => p.Medication == medication.Name && p.Visit.CompletedAt.HasValue && p.Visit.CompletedAt.Value >= usageWindowStart);

        var totalUsed = await usageQuery.SumAsync(p => (int?)p.Quantity) ?? 0;
        var lastUsed = await usageQuery
            .OrderByDescending(p => p.Visit.CompletedAt)
            .Select(p => p.Visit.CompletedAt)
            .FirstOrDefaultAsync();

        var averageUsagePerMonth = totalUsed > 0 ? Math.Max(1, totalUsed / safeMonths) : 0;
        var estimatedMonthsUntilStockout = averageUsagePerMonth > 0 ? medication.Quantity / averageUsagePerMonth : 0;

        return new AdminMedicationUsageReportDto
        {
            MedicationName = medication.Name,
            TotalUsed = totalUsed,
            AverageUsagePerMonth = averageUsagePerMonth,
            LastUsed = lastUsed ?? DateTime.UtcNow,
            CurrentStock = medication.Quantity,
            ReorderLevel = medication.ReorderLevel,
            EstimatedMonthsUntilStockout = estimatedMonthsUntilStockout
        };
    }

    // ===== AUDIT & LOGGING =====

    public async Task<List<AdminAuditLogDto>> GetAuditLogsAsync(
        int days = 30,
        string? userEmail = null,
        string? action = null)
    {
        _logger.LogInformation("Admin: Retrieving audit logs (days: {Days}, email: {Email}, action: {Action})", days, userEmail, action);

            var fromDate = DateTime.UtcNow.AddDays(-days);
        
            var query = _context.AdminAuditEvents
                .Where(a => a.OccurredAtUtc >= fromDate)
                .AsQueryable();
        
            if (!string.IsNullOrEmpty(userEmail))
                query = query.Where(a => a.PerformedByEmail == userEmail);
        
            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);
        
            var logs = await query.OrderByDescending(a => a.OccurredAtUtc).ToListAsync();
        
            return logs.Select(l => new AdminAuditLogDto
            {
                Id = l.Id,
                UserId = l.PerformedByUserId,
                UserEmail = l.PerformedByEmail,
                Action = l.Action,
                Endpoint = $"{l.TargetType}/{l.TargetId}",
                StatusCode = 200,
                Timestamp = l.OccurredAtUtc,
                Details = l.MetadataJson
            }).ToList();
    }

    public async Task<List<AdminAuditLogDto>> SearchAuditLogsAsync(AdminAuditLogFilterDto filter)
    {
        _logger.LogInformation("Admin: Searching audit logs with filter");

            var fromDate = filter.FromDate ?? DateTime.UtcNow.AddDays(-filter.Days);
            var toDate = filter.ToDate ?? DateTime.UtcNow;
        
            var query = _context.AdminAuditEvents
                .Where(a => a.OccurredAtUtc >= fromDate && a.OccurredAtUtc <= toDate)
                .AsQueryable();
        
            if (!string.IsNullOrEmpty(filter.UserEmail))
                query = query.Where(a => a.PerformedByEmail.Contains(filter.UserEmail));
        
            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(a => a.Action == filter.Action);
        
            if (!string.IsNullOrEmpty(filter.TargetType))
                query = query.Where(a => a.TargetType == filter.TargetType);
        
            var logs = await query.OrderByDescending(a => a.OccurredAtUtc).ToListAsync();
        
            return logs.Select(l => new AdminAuditLogDto
            {
                Id = l.Id,
                UserId = l.PerformedByUserId,
                UserEmail = l.PerformedByEmail,
                Action = l.Action,
                Endpoint = $"{l.TargetType}/{l.TargetId}",
                StatusCode = 200,
                Timestamp = l.OccurredAtUtc,
                Details = l.MetadataJson
            }).ToList();
    }

    public async Task<VetCleanupDryRunResponseDto> PreviewVetAccountCleanupAsync()
    {
        _logger.LogInformation("Admin: Previewing vet account cleanup candidates (dry-run)");

        // Get all vet accounts that are NOT protected
        var unprotectedVets = await _context.Owners
            .Where(o => o.Roles.Contains("Vet"))
            .Include(o => o.RefreshTokens)
            .ToListAsync();
        
        // Filter in-memory to avoid LINQ translation issues
        unprotectedVets = unprotectedVets
            .Where(o => o.VetCleanupProtected != true)
            .ToList();

        var protectedVets = await _context.Owners
            .Where(o => o.Roles.Contains("Vet") && o.VetCleanupProtected == true)
            .CountAsync();

        var candidates = new List<VetCleanupDryRunCandidateDto>();
        int totalAppointmentsImpacted = 0;
        int totalActiveTokensImpacted = 0;

        foreach (var owner in unprotectedVets)
        {
            var vetProfile = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == owner.Id);
            if (vetProfile == null) continue;

            var appointmentCount = await _context.Appointments
                .Where(a => a.VeterinarianId == owner.Id)
                .CountAsync();

            var activeTokenCount = owner.RefreshTokens
                .Where(rt => !rt.RevokedAt.HasValue && rt.Expires > DateTime.UtcNow)
                .Count();

            totalAppointmentsImpacted += appointmentCount;
            totalActiveTokensImpacted += activeTokenCount;

            // Determine reason for candidacy
            string reason = "Not protected (created outside admin-only window)";
            if (owner.VetAccountCreatedByUserId.HasValue && owner.VetAccountCreatedByUserId != Guid.Empty)
            {
                var createdByRole = owner.VetAccountCreatedByRolesCsv ?? "Unknown";
                if (!createdByRole.Contains("Admin"))
                {
                    reason = $"Non-admin creation: {createdByRole}";
                }
            }

            // Get creator email
            string createdByEmail = "Unknown";
            if (owner.VetAccountCreatedByUserId.HasValue && owner.VetAccountCreatedByUserId != Guid.Empty)
            {
                var createdByUser = await _context.Owners.FirstOrDefaultAsync(o => o.Id == owner.VetAccountCreatedByUserId);
                createdByEmail = createdByUser?.Email ?? "Unknown";
            }

            candidates.Add(new VetCleanupDryRunCandidateDto
            {
                VetAccountId = owner.Id,
                Email = owner.Email,
                FirstName = owner.FirstName ?? string.Empty,
                LastName = owner.LastName ?? string.Empty,
                LicenseNumber = vetProfile.LicenseNumber,
                CreatedAtUtc = owner.VetAccountCreatedAtUtc,
                CreatedByEmail = createdByEmail,
                Reason = reason,
                AppointmentCount = appointmentCount,
                ActiveRefreshTokenCount = activeTokenCount
            });
        }

        var previewHash = GeneratePreviewHash(candidates);

        var response = new VetCleanupDryRunResponseDto
        {
            Candidates = candidates,
            TotalCandidateCount = candidates.Count,
            TotalProtectedCount = protectedVets,
            TotalAppointmentsImpacted = totalAppointmentsImpacted,
            TotalActiveTokensImpacted = totalActiveTokensImpacted,
            PreviewGeneratedAtUtc = DateTime.UtcNow,
            PreviewHash = previewHash,
            Notes = $"Protected (safe) vet accounts: {protectedVets}. " +
                    $"Unprotected (candidate) accounts: {candidates.Count}. " +
                    $"Total vet accounts: {protectedVets + candidates.Count}."
        };

        _logger.LogInformation("Admin: Cleanup preview generated. Protected: {Protected}, Candidates: {Candidates}, Hash: {Hash}",
            protectedVets, candidates.Count, previewHash);
        return response;
    }

    private string GeneratePreviewHash(List<VetCleanupDryRunCandidateDto> candidates)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var candidateIds = string.Join(",", candidates.OrderBy(c => c.VetAccountId).Select(c => c.VetAccountId));
            var bytes = System.Text.Encoding.UTF8.GetBytes(candidateIds);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
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
