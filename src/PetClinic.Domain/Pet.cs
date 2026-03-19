using System.ComponentModel.DataAnnotations;

namespace PetClinic.Domain;

public class Pet : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(100)]
    public string Species { get; set; } = default!;

    [MaxLength(100)]
    public string Breed { get; set; } = default!;

    public DateTime? DateOfBirth { get; set; }

    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = default!;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}