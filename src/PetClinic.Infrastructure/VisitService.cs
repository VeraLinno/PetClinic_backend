using Microsoft.EntityFrameworkCore;
using PetClinic.Application;
using PetClinic.Domain;

namespace PetClinic.Infrastructure;

public class VisitService : IVisitService
{
    private readonly PetClinicDbContext _context;
    private readonly IUserContextService _userContext;

    public VisitService(PetClinicDbContext context, IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task CompleteVisitAsync(Guid visitId, VisitCompletionDto dto)
    {
        var currentUserId = _userContext.GetCurrentUserId();
        var currentUserRoles = _userContext.GetCurrentUserRoles();

        if (!currentUserRoles.Contains("Vet"))
        {
            throw new UnauthorizedAccessException("Only vets can complete visits");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var visit = await _context.Visits
                .Include(v => v.Appointment)
                .Include(v => v.Prescriptions)
                .FirstOrDefaultAsync(v => v.Id == visitId);

            if (visit == null)
            {
                throw new KeyNotFoundException("Visit not found");
            }

            // Check if vet is assigned to this appointment
            if (visit.Appointment.VeterinarianId != currentUserId)
            {
                throw new UnauthorizedAccessException("Vet not assigned to this visit");
            }

            // Mark visit complete
            visit.CompletedAt = DateTime.UtcNow;
            visit.Notes = dto.Notes;

            // Create prescriptions and check stock
            decimal totalAmount = 0;
            foreach (var prescriptionDto in dto.Prescriptions)
            {
                var stock = await _context.MedicationStocks
                    .FirstOrDefaultAsync(m => m.Name == prescriptionDto.Medication);

                if (stock == null)
                {
                    throw new InvalidOperationException($"Medication {prescriptionDto.Medication} not found in stock");
                }

                if (stock.Quantity < prescriptionDto.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for {prescriptionDto.Medication}");
                }

                // Decrement stock
                stock.Quantity -= prescriptionDto.Quantity;

                // Create prescription
                var prescription = new Prescription
                {
                    VisitId = visitId,
                    Medication = prescriptionDto.Medication,
                    Dosage = prescriptionDto.Dosage,
                    Quantity = prescriptionDto.Quantity
                };
                _context.Prescriptions.Add(prescription);

                // Calculate amount (simplified)
                totalAmount += prescriptionDto.Quantity * 10; // $10 per unit
            }

            // Create invoice
            var invoice = new Invoice
            {
                VisitId = visitId,
                Amount = totalAmount + 50, // Base visit fee $50
                IssuedAt = DateTime.UtcNow
            };
            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}