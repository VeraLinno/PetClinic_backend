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
        dto ??= new VisitCompletionDto();

        var currentUserId = _userContext.GetCurrentUserId();
        var currentUserRoles = _userContext.GetCurrentUserRoles();
        var isVet = currentUserRoles.Contains("Vet");
        var isAdmin = currentUserRoles.Contains("Admin");

        if (!isVet && !isAdmin)
        {
            throw new UnauthorizedAccessException("Only vets can complete visits");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Route ids are historically appointment ids in the UI. Support both ids safely.
            var visit = await _context.Visits
                .Include(v => v.Appointment)
                .Include(v => v.Prescriptions)
                .Include(v => v.Invoice)
                .FirstOrDefaultAsync(v => v.Id == visitId);

            if (visit == null)
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Visit)
                    .FirstOrDefaultAsync(a => a.Id == visitId);

                if (appointment == null)
                {
                    throw new KeyNotFoundException("Visit not found");
                }

                if (appointment.Visit != null)
                {
                    visit = await _context.Visits
                        .Include(v => v.Appointment)
                        .Include(v => v.Prescriptions)
                        .Include(v => v.Invoice)
                        .FirstAsync(v => v.Id == appointment.Visit.Id);
                }
                else
                {
                    visit = new Visit
                    {
                        AppointmentId = appointment.Id
                    };

                    _context.Visits.Add(visit);
                    await _context.SaveChangesAsync();

                    visit = await _context.Visits
                        .Include(v => v.Appointment)
                        .Include(v => v.Prescriptions)
                        .Include(v => v.Invoice)
                        .FirstAsync(v => v.Id == visit.Id);
                }
            }

            // Check if vet is assigned to this appointment
            if (isVet && visit.Appointment.VeterinarianId != currentUserId)
            {
                throw new UnauthorizedAccessException("Vet not assigned to this visit");
            }

            if (visit.Appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot complete a cancelled appointment");
            }

            if (visit.CompletedAt.HasValue || visit.Appointment.Status == AppointmentStatus.Completed || visit.Invoice != null)
            {
                throw new InvalidOperationException("Visit is already completed");
            }

            // Mark visit complete
            visit.CompletedAt = DateTime.UtcNow;
            visit.Notes = dto.Notes;
            visit.Appointment.Status = AppointmentStatus.Completed;

            // Create prescriptions and check stock
            decimal totalAmount = 0;
            foreach (var prescriptionDto in dto.Prescriptions ?? Enumerable.Empty<PrescriptionDto>())
            {
                if (prescriptionDto.Quantity <= 0)
                {
                    throw new InvalidOperationException("Prescription quantity must be greater than zero");
                }

                MedicationStock? stock = null;

                if (prescriptionDto.MedicationId.HasValue)
                {
                    stock = await _context.MedicationStocks
                        .FirstOrDefaultAsync(m => m.Id == prescriptionDto.MedicationId.Value);
                }

                if (stock == null && !string.IsNullOrWhiteSpace(prescriptionDto.Medication))
                {
                    var medicationName = prescriptionDto.Medication.Trim().ToLower();
                    stock = await _context.MedicationStocks
                        .FirstOrDefaultAsync(m => m.Name.ToLower() == medicationName);
                }

                if (stock == null)
                {
                    var medicationLabel = prescriptionDto.MedicationId?.ToString() ?? prescriptionDto.Medication;
                    throw new InvalidOperationException($"Medication {medicationLabel} not found in stock");
                }

                if (stock.Quantity < prescriptionDto.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for {stock.Name}");
                }

                // Decrement stock
                stock.Quantity -= prescriptionDto.Quantity;

                // Create prescription
                var prescription = new Prescription
                {
                    VisitId = visit.Id,
                    Medication = stock.Name,
                    Dosage = prescriptionDto.Dosage,
                    Quantity = prescriptionDto.Quantity
                };
                _context.Prescriptions.Add(prescription);

                // Calculate amount (simplified)
                totalAmount += prescriptionDto.Quantity * 10; // $10 per unit
            }

            var invoiceAmount = dto.InvoiceAmount;
            if (invoiceAmount <= 0)
            {
                throw new InvalidOperationException("Invoice amount must be greater than zero");
            }

            // Create invoice
            var invoice = new Invoice
            {
                VisitId = visit.Id,
                Amount = invoiceAmount,
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