using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using PetClinic.Application;
using PetClinic.Domain;
using PetClinic.Infrastructure;
using Xunit;

namespace PetClinic.Tests;

public class VisitServiceTests
{
    private readonly Mock<IUserContextService> _userContextMock;
    private readonly PetClinicDbContext _context;
    private readonly VisitService _service;

    public VisitServiceTests()
    {
        _userContextMock = new Mock<IUserContextService>();
        var options = new DbContextOptionsBuilder<PetClinicDbContext>()
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .UseInMemoryDatabase($"VisitTests-{Guid.NewGuid()}")
            .Options;
        _context = new PetClinicDbContext(options);
        _service = new VisitService(_context, _userContextMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CompleteVisitAsync_ShouldCompleteVisitAndCreateInvoice_WhenStockIsSufficient()
    {
        // Arrange
        var vetId = Guid.NewGuid();
        var owner = new Owner { Id = Guid.NewGuid(), Email = "owner@test.com", PasswordHash = "hash" };
        var pet = new Pet { Id = Guid.NewGuid(), Name = "Fluffy", Species = "Cat", Breed = "Persian", OwnerId = owner.Id };
        var vet = new Veterinarian { Id = vetId, Name = "Dr.", LastName = "Smith", Email = "vet@test.com", LicenseNumber = "VET-001" };
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PetId = pet.Id,
            VeterinarianId = vetId,
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(1),
            Status = AppointmentStatus.Scheduled
        };
        var visit = new Visit
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointment.Id,
            CreatedAt = DateTime.UtcNow
        };
        var medication = new MedicationStock
        {
            Id = Guid.NewGuid(),
            Name = "Aspirin",
            Quantity = 100
        };

        _context.Owners.Add(owner);
        _context.Pets.Add(pet);
        _context.Veterinarians.Add(vet);
        _context.Appointments.Add(appointment);
        _context.Visits.Add(visit);
        _context.MedicationStocks.Add(medication);
        await _context.SaveChangesAsync();

        _userContextMock.Setup(u => u.GetCurrentUserId()).Returns(vetId);
        _userContextMock.Setup(u => u.GetCurrentUserRoles()).Returns(new List<string> { "Vet" });

        var dto = new VisitCompletionDto
        {
            Notes = "Visit completed successfully",
            InvoiceAmount = 150,
            Prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto { Medication = "Aspirin", Dosage = "100mg", Quantity = 10 }
            }
        };

        // Act
        await _service.CompleteVisitAsync(visit.Id, dto);

        // Assert
        var updatedVisit = await _context.Visits.FindAsync(visit.Id);
        updatedVisit.Should().NotBeNull();
        updatedVisit!.CompletedAt.Should().NotBeNull();
        updatedVisit.Notes.Should().Be("Visit completed successfully");

        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.VisitId == visit.Id);
        invoice.Should().NotBeNull();
        invoice!.Amount.Should().Be(150); // 50 base + 10 * 10

        var updatedStock = await _context.MedicationStocks.FindAsync(medication.Id);
        updatedStock!.Quantity.Should().Be(90);

        var prescription = await _context.Prescriptions.FirstOrDefaultAsync(p => p.VisitId == visit.Id);
        prescription.Should().NotBeNull();
        prescription!.Medication.Should().Be("Aspirin");
        prescription.Quantity.Should().Be(10);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CompleteVisitAsync_ShouldThrowException_WhenStockIsInsufficient()
    {
        // Arrange
        var vetId = Guid.NewGuid();
        var owner = new Owner { Id = Guid.NewGuid(), Email = "owner@test.com", PasswordHash = "hash" };
        var pet = new Pet { Id = Guid.NewGuid(), Name = "Fluffy", Species = "Cat", Breed = "Persian", OwnerId = owner.Id };
        var vet = new Veterinarian { Id = vetId, Name = "Dr.", LastName = "Smith", Email = "vet@test.com", LicenseNumber = "VET-001" };
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PetId = pet.Id,
            VeterinarianId = vetId,
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(1),
            Status = AppointmentStatus.Scheduled
        };
        var visit = new Visit
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointment.Id,
            CreatedAt = DateTime.UtcNow
        };
        var medication = new MedicationStock
        {
            Id = Guid.NewGuid(),
            Name = "Aspirin",
            Quantity = 5 // Insufficient
        };

        _context.Owners.Add(owner);
        _context.Pets.Add(pet);
        _context.Veterinarians.Add(vet);
        _context.Appointments.Add(appointment);
        _context.Visits.Add(visit);
        _context.MedicationStocks.Add(medication);
        await _context.SaveChangesAsync();

        _userContextMock.Setup(u => u.GetCurrentUserId()).Returns(vetId);
        _userContextMock.Setup(u => u.GetCurrentUserRoles()).Returns(new List<string> { "Vet" });

        var dto = new VisitCompletionDto
        {
            InvoiceAmount = 150,
            Prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto { Medication = "Aspirin", Dosage = "100mg", Quantity = 10 }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteVisitAsync(visit.Id, dto));

        // Verify rollback - stock should remain unchanged
        var stockAfter = await _context.MedicationStocks.FindAsync(medication.Id);
        stockAfter!.Quantity.Should().Be(5);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CompleteVisitAsync_ShouldThrowException_WhenUserIsNotVet()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var owner = new Owner { Id = ownerId, Email = "owner-authz@test.com", PasswordHash = "hash" };
        var vet = new Veterinarian { Id = Guid.NewGuid(), Name = "Dr.", LastName = "Assigned", Email = "assigned-vet@test.com", LicenseNumber = "VET-ASSIGNED" };
        var pet = new Pet { Id = Guid.NewGuid(), Name = "Rex", Species = "Dog", Breed = "Mixed", OwnerId = ownerId };
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PetId = pet.Id,
            VeterinarianId = vet.Id,
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(1),
            Status = AppointmentStatus.Scheduled
        };
        var visit = new Visit { Id = Guid.NewGuid(), AppointmentId = appointment.Id, CreatedAt = DateTime.UtcNow };

        _context.Owners.Add(owner);
        _context.Veterinarians.Add(vet);
        _context.Pets.Add(pet);
        _context.Appointments.Add(appointment);
        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();

        _userContextMock.Setup(u => u.GetCurrentUserId()).Returns(ownerId);
        _userContextMock.Setup(u => u.GetCurrentUserRoles()).Returns(new List<string> { "Owner" });

        var dto = new VisitCompletionDto { InvoiceAmount = 100, Prescriptions = new List<PrescriptionDto>() };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CompleteVisitAsync(visit.Id, dto));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CompleteVisitAsync_ShouldResolveMedicationById_WhenMedicationIdProvided()
    {
        // Arrange
        var vetId = Guid.NewGuid();
        var owner = new Owner { Id = Guid.NewGuid(), Email = "owner-id@test.com", PasswordHash = "hash" };
        var pet = new Pet { Id = Guid.NewGuid(), Name = "Fluffy", Species = "Cat", Breed = "Persian", OwnerId = owner.Id };
        var vet = new Veterinarian { Id = vetId, Name = "Dr.", LastName = "Smith", Email = "vet-id@test.com", LicenseNumber = "VET-002" };
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PetId = pet.Id,
            VeterinarianId = vetId,
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(1),
            Status = AppointmentStatus.Scheduled
        };
        var visit = new Visit
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointment.Id,
            CreatedAt = DateTime.UtcNow
        };
        var medication = new MedicationStock
        {
            Id = Guid.NewGuid(),
            Name = "Amoxicillin",
            Quantity = 50
        };

        _context.Owners.Add(owner);
        _context.Pets.Add(pet);
        _context.Veterinarians.Add(vet);
        _context.Appointments.Add(appointment);
        _context.Visits.Add(visit);
        _context.MedicationStocks.Add(medication);
        await _context.SaveChangesAsync();

        _userContextMock.Setup(u => u.GetCurrentUserId()).Returns(vetId);
        _userContextMock.Setup(u => u.GetCurrentUserRoles()).Returns(new List<string> { "Vet" });

        var dto = new VisitCompletionDto
        {
            Notes = "Visit completed by medication id",
            InvoiceAmount = 220,
            Prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto { MedicationId = medication.Id, Medication = "", Dosage = "250mg", Quantity = 5 }
            }
        };

        // Act
        await _service.CompleteVisitAsync(visit.Id, dto);

        // Assert
        var updatedStock = await _context.MedicationStocks.FindAsync(medication.Id);
        updatedStock.Should().NotBeNull();
        updatedStock!.Quantity.Should().Be(45);

        var savedPrescription = await _context.Prescriptions.FirstOrDefaultAsync(p => p.VisitId == visit.Id);
        savedPrescription.Should().NotBeNull();
        savedPrescription!.Medication.Should().Be("Amoxicillin");
        savedPrescription.Quantity.Should().Be(5);
    }
}