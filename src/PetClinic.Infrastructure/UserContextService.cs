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
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  
        return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;  
    }  
  
    public List<string> GetCurrentUserRoles()  
    {  
        var rolesClaim = _httpContextAccessor.HttpContext?.User.FindFirst("roles")?.Value;  
        if (string.IsNullOrEmpty(rolesClaim))  
            return new List<string>();  
  
        return rolesClaim.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();  
    }  
} 
