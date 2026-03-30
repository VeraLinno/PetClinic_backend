namespace PetClinic.Application;

/// <summary>
/// Administrative service for system management operations.
/// Only accessible to users with Admin role.
/// </summary>
public interface IAdminService
{
    // ===== USER MANAGEMENT =====

    /// <summary>
    /// Get all users in the system.
    /// </summary>
    Task<List<AdminUserDto>> GetAllUsersAsync();

    /// <summary>
    /// Get a specific user by ID.
    /// </summary>
    Task<AdminUserDto?> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Update user roles and status.
    /// </summary>
    Task<AdminUserDto> UpdateUserAsync(Guid userId, UpdateUserRoleDto dto);

    /// <summary>
    /// Delete a user (soft delete - mark as inactive).
    /// </summary>
    Task DeleteUserAsync(Guid userId);

    /// <summary>
    /// Get user login history.
    /// </summary>
    Task<List<AdminAuditLogDto>> GetUserActivityAsync(Guid userId, int days = 30);

    // ===== VETERINARIAN MANAGEMENT =====

    /// <summary>
    /// Get all veterinarians.
    /// </summary>
    Task<List<AdminVeterinarianDto>> GetAllVeterinariansAsync();

    /// <summary>
    /// Get a specific veterinarian by ID.
    /// </summary>
    Task<AdminVeterinarianDto?> GetVeterinarianByIdAsync(Guid vetId);

    /// <summary>
    /// Deactivate a veterinarian.
    /// </summary>
    Task DeactivateVeterinarianAsync(Guid vetId);

    /// <summary>
    /// Reactivate a veterinarian.
    /// </summary>
    Task ReactivateVeterinarianAsync(Guid vetId);

    // ===== APPOINTMENT MANAGEMENT =====

    /// <summary>
    /// Get all appointments (with optional filters).
    /// </summary>
    Task<List<AdminAppointmentDto>> GetAllAppointmentsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? status = null);

    /// <summary>
    /// Force cancel an appointment (admin override).
    /// </summary>
    Task CancelAppointmentAsync(Guid appointmentId, string reason);

    /// <summary>
    /// Reassign appointment to different veterinarian.
    /// </summary>
    Task ReassignAppointmentAsync(Guid appointmentId, Guid newVetId);

    // ===== FINANCIAL MANAGEMENT =====

    /// <summary>
    /// Get all invoices (with optional date filter).
    /// </summary>
    Task<List<InvoiceDto>> GetAllInvoicesAsync(DateTime? fromDate = null, DateTime? toDate = null);

    /// <summary>
    /// Get financial report for period.
    /// </summary>
    Task<AdminFinancialReportDto> GetFinancialReportAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Adjust invoice amount (e.g., apply discount, correction).
    /// </summary>
    Task AdjustInvoiceAsync(Guid invoiceId, decimal newAmount, string reason);

    // ===== INVENTORY MANAGEMENT =====

    /// <summary>
    /// Get inventory report with usage statistics.
    /// </summary>
    Task<List<AdminInventoryReportDto>> GetInventoryReportAsync();

    /// <summary>
    /// Get medications with low stock.
    /// </summary>
    Task<List<AdminInventoryReportDto>> GetLowStockMedicationsAsync();

    /// <summary>
    /// Get medication usage report.
    /// </summary>
    Task<AdminMedicationUsageReportDto> GetMedicationUsageAsync(Guid medicationId, int months = 3);

    // ===== AUDIT &LOGGING =====

    /// <summary>
    /// Get audit logs (all system activity).
    /// </summary>
    Task<List<AdminAuditLogDto>> GetAuditLogsAsync(
        int days = 30,
        string? userEmail = null,
        string? action = null);

    /// <summary>
    /// Search audit logs by criteria.
    /// </summary>
    Task<List<AdminAuditLogDto>> SearchAuditLogsAsync(AdminAuditLogFilterDto filter);

    // ===== DASHBOARD METRICS =====

    /// <summary>
    /// Get dashboard overview metrics.
    /// </summary>
    Task<AdminDashboardMetricsDto> GetDashboardMetricsAsync();

    /// <summary>
    /// Get system health status.
    /// </summary>
    Task<AdminSystemHealthDto> GetSystemHealthAsync();
}

// Additional DTOs for admin service

public class AdminFinancialReportDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal OutstandingAmount { get; set; }
    public decimal AverageInvoiceAmount { get; set; }
}

public class AdminMedicationUsageReportDto
{
    public string MedicationName { get; set; } = default!;
    public int TotalUsed { get; set; }
    public int AverageUsagePerMonth { get; set; }
    public DateTime LastUsed { get; set; }
    public int CurrentStock { get; set; }
    public int ReorderLevel { get; set; }
    public int EstimatedMonthsUntilStockout { get; set; }
}

public class AdminAuditLogFilterDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? UserEmail { get; set; }
    public string? Action { get; set; }
    public int? StatusCode { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class AdminSystemHealthDto
{
    public string Status { get; set; } = default!;  // "Healthy", "Warning", "Critical"
    public DateTime CheckedAt { get; set; }
    public int ActiveUsers { get; set; }
    public int DatabaseConnections { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public string? Issues { get; set; }
}
