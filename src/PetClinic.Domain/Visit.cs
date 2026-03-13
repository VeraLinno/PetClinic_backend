namespace PetClinic.Domain;

public class Visit : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = default!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public Invoice? Invoice { get; set; }
}