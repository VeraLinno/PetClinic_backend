namespace PetClinic.Application;

public interface IUserContextService
{
    Guid GetCurrentUserId();
    List<string> GetCurrentUserRoles();
}