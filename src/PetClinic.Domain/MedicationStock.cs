using System.ComponentModel.DataAnnotations;

namespace PetClinic.Domain;

public class MedicationStock : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(50)]
    public string Category { get; set; } = "Medications";

    [Range(0, 9999999.99)]
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    [MaxLength(20)]
    public string Unit { get; set; } = "units";

    public int ReorderLevel { get; set; } = 10;

    public ICollection<InventoryReorder> Reorders { get; set; } = new List<InventoryReorder>();
}