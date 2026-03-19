using System.ComponentModel.DataAnnotations;

namespace PetClinic.Application;

public class OwnerDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class PetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Species { get; set; } = default!;
    public string Breed { get; set; } = default!;
    public DateTime? DateOfBirth { get; set; }
    public Guid OwnerId { get; set; }
}

public class CreatePetDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string Species { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string Breed { get; set; } = default!;

    public DateTime? DateOfBirth { get; set; }
}

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public string? PetName { get; set; }
    public Guid? VeterinarianId { get; set; }
    public string? VeterinarianName { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Status { get; set; } = default!;
}

public class VeterinarianOptionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
}

public class CreateAppointmentDto
{
    [Required]
    public Guid PetId { get; set; }

    public Guid? VeterinarianId { get; set; }

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
    public string Unit { get; set; } = default!;
    public int ReorderLevel { get; set; }
}

public class ReorderMedicationRequestDto
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class ReorderMedicationResponseDto
{
    public Guid MedicationId { get; set; }
    public string MedicationName { get; set; } = default!;
    public int OrderedQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public DateTime DeliveryAtUtc { get; set; }
    public string Message { get; set; } = default!;
}