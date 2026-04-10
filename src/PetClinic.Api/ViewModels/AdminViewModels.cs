using PetClinic.Application;

namespace PetClinic.Api.ViewModels;

public class AdminDashboardPageViewModel
{
    public AdminDashboardMetricsDto? Metrics { get; set; }
    public AdminSystemHealthDto? Health { get; set; }
}

public class AdminAuditIndexPageViewModel
{
    public List<AdminAuditLogDto> Logs { get; set; } = new();
    public int Days { get; set; } = 30;
    public string? UserEmail { get; set; }
    public string? Action { get; set; }
}

public class AdminAuditUserActivityPageViewModel
{
    public Guid UserId { get; set; }
    public int Days { get; set; } = 30;
    public List<AdminAuditLogDto> Activities { get; set; } = new();
}

public class AdminAuditSearchPageViewModel
{
    public AdminAuditLogFilterDto Filter { get; set; } = new();
    public List<AdminAuditLogDto> Results { get; set; } = new();
}

public class AdminAppointmentReassignPageViewModel
{
    public AdminAppointmentDto? Appointment { get; set; }
    public List<AdminVeterinarianDto> Veterinarians { get; set; } = new();
}
