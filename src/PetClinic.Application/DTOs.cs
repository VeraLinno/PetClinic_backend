using System.ComponentModel.DataAnnotations;

namespace PetClinic.Application;

public class OwnerDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public List<string> Roles { get; set; } = new();
}

public class PetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Species { get; set; } = default!;
    public Guid OwnerId { get; set; }
}

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Guid VeterinarianId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Status { get; set; } = default!;
}

public class CreateAppointmentDto
{
    [Required]
    public Guid PetId { get; set; }

    [Required]
    public Guid VeterinarianId { get; set; }

    [Required]
    public DateTime StartAt { get; set; }

    [Required]
    public DateTime EndAt { get; set; }
}

public class VisitDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class VisitCompletionDto
{
    public string? Notes { get; set; }
    public List<PrescriptionDto> Prescriptions { get; set; } = new();
}

public class PrescriptionDto
{
    public string Medication { get; set; } = default!;
    public string Dosage { get; set; } = default!;
    public int Quantity { get; set; }
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid VisitId { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssuedAt { get; set; }
}

public class MedicationStockDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
}