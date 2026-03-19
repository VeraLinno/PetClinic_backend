namespace PetClinic.Domain;

public class InventoryReorder : BaseEntity
{
    public Guid MedicationStockId { get; set; }
    public MedicationStock MedicationStock { get; set; } = default!;

    public int Quantity { get; set; }
    public DateTime ScheduledForUtc { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }

    public Guid? OrderedByVetId { get; set; }
}
