using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using PetClinic.Application;
using PetClinic.Domain;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly IUserContextService _userContext;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;

    public AppointmentsController(IAppointmentService appointmentService, IUserContextService userContext, IMapper mapper, ILocalizationService localizationService)
    {
        _appointmentService = appointmentService;
        _userContext = userContext;
        _mapper = mapper;
        _localizationService = localizationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var language = _localizationService.GetCurrentLanguage();
            var appointment = await _appointmentService.CreateAsync(dto);
            var result = _mapper.Map<AppointmentDto>(appointment);
            ApplyAppointmentLocalization(result, language);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, result);
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

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? date = null, [FromQuery] string? ownerId = null, [FromQuery] string? vetId = null)
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();
        var language = _localizationService.GetCurrentLanguage();

        var normalizedDate = NormalizeNullable(date);
        var ownerGuid = ParseGuidOrNull(ownerId);
        var vetGuid = ParseGuidOrNull(vetId);

        var appointments = await _appointmentService.GetUserAppointmentsAsync(userId, roles, normalizedDate, ownerGuid, vetGuid);
        var dtos = _mapper.Map<List<AppointmentDto>>(appointments);
        ApplyAppointmentLocalization(dtos, language);
        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();
        var language = _localizationService.GetCurrentLanguage();

        var appointments = await _appointmentService.GetUserAppointmentsAsync(userId, roles);
        var appointment = appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
        {
            return NotFound();
        }

        var dto = _mapper.Map<AppointmentDto>(appointment);
        ApplyAppointmentLocalization(dto, language);
        return Ok(dto);
    }

    private void ApplyAppointmentLocalization(IEnumerable<AppointmentDto> appointments, string language)
    {
        foreach (var appointment in appointments)
        {
            ApplyAppointmentLocalization(appointment, language);
        }
    }

    private void ApplyAppointmentLocalization(AppointmentDto appointment, string language)
    {
        appointment.StatusLocalized = _localizationService.LocalizeAppointmentStatus(appointment.Status, language);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            await _appointmentService.CancelAsync(id);
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

    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        try
        {
            await _appointmentService.ConfirmAsync(id);
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

    private static Guid? ParseGuidOrNull(string? value)
    {
        var normalized = NormalizeNullable(value);
        if (string.IsNullOrEmpty(normalized))
        {
            return null;
        }

        return Guid.TryParse(normalized, out var guid) ? guid : null;
    }

    private static string? NormalizeNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Equals("null", StringComparison.OrdinalIgnoreCase) ||
               trimmed.Equals("undefined", StringComparison.OrdinalIgnoreCase)
            ? null
            : trimmed;
    }
}
