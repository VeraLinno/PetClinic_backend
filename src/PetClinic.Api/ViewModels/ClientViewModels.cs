using PetClinic.Application;

namespace PetClinic.Api.ViewModels;

public class ClientDashboardViewModel
{
    public string OwnerName { get; set; } = "Owner";
    public int PetsCount { get; set; }
    public int AppointmentsCount { get; set; }
    public int InvoicesCount { get; set; }
    public Dictionary<string, string> UiTexts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class ClientPetsPageViewModel
{
    public List<PetDto> Pets { get; set; } = new();
    public Dictionary<string, string> UiTexts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class ClientAppointmentsPageViewModel
{
    public List<AppointmentDto> Appointments { get; set; } = new();
    public Dictionary<string, string> UiTexts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class ClientInvoicesPageViewModel
{
    public List<InvoiceDto> Invoices { get; set; } = new();
    public Dictionary<string, string> UiTexts { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
