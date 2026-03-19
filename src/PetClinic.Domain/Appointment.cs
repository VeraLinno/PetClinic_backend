namespace PetClinic.Domain;

public class Appointment : BaseEntity
{
    public Guid PetId { get; set; }
    public Pet Pet { get; set; } = default!;

    public Guid? VeterinarianId { get; set; }
    public Veterinarian? Veterinarian { get; set; }

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public Visit? Visit { get; set; }
}

public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled
}