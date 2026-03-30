using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetClinic.Domain;

public class VetUnavailability : BaseEntity
{
    [Required]
    [ForeignKey(nameof(Veterinarian))]
    public Guid VeterinarianId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [MaxLength(255)]
    public string? Reason { get; set; }

    public Veterinarian Veterinarian { get; set; } = null!;
}
