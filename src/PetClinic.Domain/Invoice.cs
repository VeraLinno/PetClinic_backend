namespace PetClinic.Domain;

public class Invoice : BaseEntity
{
    public Guid VisitId { get; set; }
    public Visit Visit { get; set; } = default!;

    public decimal Amount { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
}