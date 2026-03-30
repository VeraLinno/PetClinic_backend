using System.ComponentModel.DataAnnotations;

namespace PetClinic.Domain;

public class Veterinarian : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = default!;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = default!;

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = default!;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<VetUnavailability> Unavailabilities { get; set; } = new List<VetUnavailability>();
}