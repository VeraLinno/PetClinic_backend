using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetClinic.Application;
using PetClinic.Domain;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/v1/appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly IUserContextService _userContext;
    private readonly IMapper _mapper;

    public AppointmentsController(IAppointmentService appointmentService, IUserContextService userContext, IMapper mapper)
    {
        _appointmentService = appointmentService;
        _userContext = userContext;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var appointment = await _appointmentService.CreateAsync(dto);
            var result = _mapper.Map<AppointmentDto>(appointment);
            return CreatedAtAction(nameof(Get), new { id = appointment.Id }, result);
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
    public async Task<IActionResult> Get([FromQuery] string? date = null, [FromQuery] Guid? ownerId = null, [FromQuery] Guid? vetId = null)
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();

        var appointments = await _appointmentService.GetUserAppointmentsAsync(userId, roles, date, ownerId, vetId);
        var dtos = _mapper.Map<List<AppointmentDto>>(appointments);
        return Ok(dtos);
    }
}