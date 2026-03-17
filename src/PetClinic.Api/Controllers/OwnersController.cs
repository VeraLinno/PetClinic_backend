using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetClinic.Application;
using PetClinic.Infrastructure;  
  
namespace PetClinic.Api.Controllers;  
  
[ApiController]  
[Route("api/v1/owners")]  
[Authorize]  
public class OwnersController : ControllerBase  
{  
    private readonly PetClinicDbContext _context;  
    private readonly IUserContextService _userContext;  
    private readonly IMapper _mapper;  
  
    public OwnersController(PetClinicDbContext context, IUserContextService userContext, IMapper mapper)  
    {  
        _context = context;  
        _userContext = userContext;  
        _mapper = mapper;  
    }  
  
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = _userContext.GetCurrentUserId();
        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == userId);
        if (owner == null) return NotFound();
        var dto = _mapper.Map<OwnerDto>(owner);
        return Ok(dto);
    }
}  
