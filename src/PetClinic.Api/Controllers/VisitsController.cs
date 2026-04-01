using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using PetClinic.Application;
using PetClinic.Domain;
using PetClinic.Infrastructure;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;
    private readonly PetClinicDbContext _context;
    private readonly IUserContextService _userContext;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;

    public VisitsController(IVisitService visitService, PetClinicDbContext context, IUserContextService userContext, IMapper mapper, ILocalizationService localizationService)
    {
        _visitService = visitService;
        _context = context;
        _userContext = userContext;
        _mapper = mapper;
        _localizationService = localizationService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();
        var language = _localizationService.GetCurrentLanguage();

        var visit = await _context.Visits
            .Include(v => v.Appointment)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (visit == null)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Visit)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment?.Visit == null)
            {
                return NotFound();
            }

            visit = await _context.Visits
                .Include(v => v.Appointment)
                .FirstOrDefaultAsync(v => v.Id == appointment.Visit.Id);
        }

        if (visit == null || visit.Appointment == null)
        {
            return NotFound();
        }

        var isVet = roles.Contains("Vet");
        var canAccess = isVet
            ? visit.Appointment.VeterinarianId == userId
            : await _context.Pets.AnyAsync(p => p.Id == visit.Appointment.PetId && p.OwnerId == userId);

        if (!canAccess)
        {
            return Forbid();
        }

        var dto = _mapper.Map<VisitDto>(visit);
        dto.Status = visit.CompletedAt.HasValue || visit.Appointment.Status == AppointmentStatus.Completed
            ? "Completed"
            : "Open";
        dto.StatusLocalized = _localizationService.LocalizeVisitStatus(dto.Status, language);

        return Ok(dto);
    }

    [HttpPatch("{id}/complete")]
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
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}