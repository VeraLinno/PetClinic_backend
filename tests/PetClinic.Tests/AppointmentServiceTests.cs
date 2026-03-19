using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PetClinic.Application;
using PetClinic.Domain;
using PetClinic.Infrastructure;
using Xunit;

namespace PetClinic.Tests;

public class AppointmentServiceTests
{
    private readonly Mock<IUserContextService> _userContextMock;
    private readonly PetClinicDbContext _context;
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _userContextMock = new Mock<IUserContextService>();
        var options = new DbContextOptionsBuilder<PetClinicDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        _context = new PetClinicDbContext(options);
        _service = new AppointmentService(_context, _userContextMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreateAsync_ShouldCreateAppointment_WhenVetIsAvailable()
    {
        // Arrange
        var owner = new Owner { Id = Guid.NewGuid(), Email = "owner@test.com" };
        var pet = new Pet { Id = Guid.NewGuid(), Name = "Fluffy", OwnerId = owner.Id };
        var vet = new Veterinarian { Id = Guid.NewGuid(), Name = "Dr. Smith", Email = "vet@test.com" };

        _context.Owners.Add(owner);
        _context.Pets.Add(pet);
        _context.Veterinarians.Add(vet);
        await _context.SaveChangesAsync();

        _userContextMock.Setup(u => u.GetCurrentUserId()).Returns(owner.Id);

        var dto = new CreateAppointmentDto
        {
            PetId = pet.Id,
            VeterinarianId = vet.Id,
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.PetId.Should().Be(pet.Id);
        result.VeterinarianId.Should().Be(vet.Id);
        result.Status.Should().Be(AppointmentStatus.Scheduled);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreateAsync_ShouldThrowException_WhenVetIsNotAvailable()
    {
        // Arrange
        var owner = new Owner { Id = Guid.NewGuid(), Email = "owner@test.com" };
        var pet = new Pet { Id = Guid.NewGuid(), Name = "Fluffy", OwnerId = owner.Id };
        var vet = new Veterinarian { Id = Guid.NewGuid(), Name = "Dr. Smith", Email = "vet@test.com" };

        _context.Owners.Add(owner);
        _context.Pets.Add(pet);
        _context.Veterinarians.Add(vet);

        // Existing appointment
        var existingAppointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PetId = pet.Id,
            VeterinarianId = vet.Id,
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2),
            Status = AppointmentStatus.Scheduled
        };
        _context.Appointments.Add(existingAppointment);
        await _context.SaveChangesAsync();

        _userContextMock.Setup(u => u.GetCurrentUserId()).Returns(owner.Id);

        var dto = new CreateAppointmentDto
        {
            PetId = pet.Id,
            VeterinarianId = vet.Id,
            StartAt = DateTime.UtcNow.AddHours(1).AddMinutes(30), // Overlapping
            EndAt = DateTime.UtcNow.AddHours(2).AddMinutes(30)
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreateAsync_ShouldThrowException_WhenPetDoesNotBelongToUser()
    {
        // Arrange
        var owner = new Owner { Id = Guid.NewGuid(), Email = "owner@test.com" };
        var otherOwner = new Owner { Id = Guid.NewGuid(), Email = "other@test.com" };
        var pet = new Pet { Id = Guid.NewGuid(), Name = "Fluffy", OwnerId = otherOwner.Id };
        var vet = new Veterinarian { Id = Guid.NewGuid(), Name = "Dr. Smith", Email = "vet@test.com" };

        _context.Owners.AddRange(owner, otherOwner);
        _context.Pets.Add(pet);
        _context.Veterinarians.Add(vet);
        await _context.SaveChangesAsync();

        _userContextMock.Setup(u => u.GetCurrentUserId()).Returns(owner.Id);

        var dto = new CreateAppointmentDto
        {
            PetId = pet.Id,
            VeterinarianId = vet.Id,
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetUserAppointmentsAsync_ShouldIgnoreInvalidDateFilter()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var owner = new Owner { Id = ownerId, Email = "owner-date@test.com" };
        var pet = new Pet { Id = Guid.NewGuid(), Name = "Shadow", OwnerId = ownerId };
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PetId = pet.Id,
            StartAt = DateTime.UtcNow.AddHours(1),
            EndAt = DateTime.UtcNow.AddHours(2),
            Status = AppointmentStatus.Scheduled
        };

        _context.Owners.Add(owner);
        _context.Pets.Add(pet);
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        var roles = new List<string> { "Owner" };

        // Act
        var result = await _service.GetUserAppointmentsAsync(ownerId, roles, "not-a-date");

        // Assert
        result.Should().Contain(a => a.Id == appointment.Id);
    }
}
