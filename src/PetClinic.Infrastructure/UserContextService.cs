using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PetClinic.Application;

namespace PetClinic.Infrastructure;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? user?.FindFirst("sub")?.Value;

        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    public List<string> GetCurrentUserRoles()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var roles = user?.FindAll("roles")
                        .SelectMany(c => c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                        .ToList() ?? new List<string>();

        if (roles.Count > 0)
        {
            return roles;
        }

        return user?.FindAll(ClaimTypes.Role)
                   .Select(c => c.Value)
                   .ToList() ?? new List<string>();
    }
}
