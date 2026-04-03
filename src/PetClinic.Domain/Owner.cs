using System.ComponentModel.DataAnnotations;

namespace PetClinic.Domain;

public class Owner : BaseEntity
{
    [MaxLength(256)]
    public string Email { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    public List<string> Roles { get; set; } = new();

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public DateTime? LastLoginAt { get; set; }


    // Vet Account Audit Metadata
    public DateTime? VetAccountCreatedAtUtc { get; set; }
    public Guid? VetAccountCreatedByUserId { get; set; }
    [MaxLength(500)]
    public string? VetAccountCreatedByRolesCsv { get; set; }

    public DateTime? VetAccountUpdatedAtUtc { get; set; }
    public Guid? VetAccountUpdatedByUserId { get; set; }
    [MaxLength(500)]
    public string? VetAccountUpdatedByRolesCsv { get; set; }

    // Protection flag to prevent cleanup of existing accounts
    public bool VetCleanupProtected { get; set; } = false;
}