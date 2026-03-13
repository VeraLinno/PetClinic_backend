using System.ComponentModel.DataAnnotations;

namespace PetClinic.Domain;

public class Owner : BaseEntity
{
    [MaxLength(256)]
    public string Email { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public List<string> Roles { get; set; } = new();

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}