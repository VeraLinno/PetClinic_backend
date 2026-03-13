using System.ComponentModel.DataAnnotations;

namespace PetClinic.Domain;

public class MedicationStock : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    public int Quantity { get; set; }
}