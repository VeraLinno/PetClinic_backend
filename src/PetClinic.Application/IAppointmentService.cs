using PetClinic.Domain;

namespace PetClinic.Application;

public interface IAppointmentService
{
    Task<Appointment> CreateAsync(CreateAppointmentDto dto);
    Task<IEnumerable<Appointment>> GetUserAppointmentsAsync(Guid userId, List<string> roles, string? date = null, Guid? ownerId = null, Guid? vetId = null);
}