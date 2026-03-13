using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetClinic.Application;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/v1/visits")]
[Authorize]
public class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitsController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Vet")]
    public async Task<IActionResult> CompleteVisit(Guid id, [FromBody] VisitCompletionDto dto)
    {
        try
        {
            await _visitService.CompleteVisitAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}