namespace PetClinic.Domain;

public class RefreshToken : BaseEntity
{
    public string TokenHash { get; set; } = default!;

    public DateTime Expires { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = default!;
}