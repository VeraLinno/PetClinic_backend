using System.ComponentModel.DataAnnotations;

namespace PetClinic.Domain;

public class Prescription : BaseEntity
{
    public Guid VisitId { get; set; }
    public Visit Visit { get; set; } = default!;

    [MaxLength(100)]
    public string Medication { get; set; } = default!;

    [MaxLength(50)]
    public string Dosage { get; set; } = default!;

    public int Quantity { get; set; }
}