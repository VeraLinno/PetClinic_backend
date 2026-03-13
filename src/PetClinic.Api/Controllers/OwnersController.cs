using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetClinic.Application;

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
        var owner = await _context.Owners.FindAsync(userId);

        if (owner == null)
        {
            return NotFound();
        }

        var dto = _mapper.Map<OwnerDto>(owner);
        return Ok(dto);
    }

    [HttpGet("{id}/pets")]
    public async Task<IActionResult> GetPets(Guid id)
    {
        var currentUserId = _userContext.GetCurrentUserId();
        var currentUserRoles = _userContext.GetCurrentUserRoles();

        // Allow if requesting own pets or if vet
        if (id != currentUserId && !currentUserRoles.Contains("Vet"))
        {
            return Forbid();
        }

        var pets = await _context.Pets
            .Where(p => p.OwnerId == id)
            .ToListAsync();

        var dtos = _mapper.Map<List<PetDto>>(pets);
        return Ok(dtos);
    }
}