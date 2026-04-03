namespace PetClinic.Domain;

public class Invoice : BaseEntity
{
    public Guid VisitId { get; set; }
    public Visit Visit { get; set; } = default!;

    public decimal Amount { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public enum PaymentStatus
    {
        Unpaid = 0,
        Paid = 1,
        Overdue = 2
    }
    
    public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;
    public DateTime? PaidAt { get; set; }
    public DateTime? DueDate { get; set; } = DateTime.UtcNow.AddDays(30);
}