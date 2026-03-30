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

public class UpdateOwnerProfileDto
{
    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }
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

public class VetAccountDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string? PhoneNumber { get; set; }
}

public class UpdateVetAccountDto
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = default!;

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
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
    public string Category { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; } = default!;
    public int ReorderLevel { get; set; }
}

public class UpdateMedicationStockDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = default!;

    [Range(0, 9999999.99)]
    public decimal UnitPrice { get; set; }
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

public class PendingInventoryReorderDto
{
    public Guid ReorderId { get; set; }
    public Guid MedicationId { get; set; }
    public string MedicationName { get; set; } = default!;
    public int Quantity { get; set; }
    public DateTime DeliveryAtUtc { get; set; }
}

public class DeliveredInventoryReorderDto
{
    public Guid ReorderId { get; set; }
    public Guid MedicationId { get; set; }
    public string MedicationName { get; set; } = default!;
    public int Quantity { get; set; }
    public DateTime DeliveryAtUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
}

// ========== ADMIN DTOs ==========

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AdminVeterinarianDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public int TotalAppointments { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AdminAppointmentDto
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public string? PetName { get; set; }
    public string? OwnerEmail { get; set; }
    public Guid? VeterinarianId { get; set; }
    public string? VeterinarianName { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Status { get; set; } = default!;
}

public class AdminAuditLogDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string Endpoint { get; set; } = default!;
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}

public class AdminDashboardMetricsDto
{
    public int TotalUsers { get; set; }
    public int TotalVeterinarians { get; set; }
    public int TotalPets { get; set; }
    public int TotalAppointmentsThisMonth { get; set; }
    public int CompletedVisitsThisMonth { get; set; }
    public decimal TotalRevenueThisMonth { get; set; }
    public int LowStockMedications { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class AdminInventoryReportDto
{
    public Guid MedicationId { get; set; }
    public string MedicationName { get; set; } = default!;
    public string Category { get; set; } = default!;
    public int CurrentQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public string Unit { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int UsedThisMonth { get; set; }
    public bool IsLowStock { get; set; }
}

public class UpdateUserRoleDto
{
    [Required]
    public List<string> Roles { get; set; } = new();

    public bool IsActive { get; set; } = true;
}