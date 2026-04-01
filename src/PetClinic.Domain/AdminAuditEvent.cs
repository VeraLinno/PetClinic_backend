using System.ComponentModel.DataAnnotations;

namespace PetClinic.Domain;

public class AdminAuditEvent : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = default!; // e.g., "VetAccountCreated", "VetAccountDeleteRequested", "VetCleanupDryRunExecuted"

    [Required]
    [MaxLength(100)]
    public string TargetType { get; set; } = default!; // e.g., "VetAccount", "Owner", "Veterinarian"

    [Required]
    public Guid TargetId { get; set; }

    [Required]
    public Guid PerformedByUserId { get; set; }

    [Required]
    [MaxLength(256)]
    public string PerformedByEmail { get; set; } = default!;

    [Required]
    [MaxLength(500)]
    public string PerformedByRolesCsv { get; set; } = default!;

    [Required]
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

    // JSON-serialized additional context (e.g., candidate count, impact summary, reason)
    public string? MetadataJson { get; set; }
}