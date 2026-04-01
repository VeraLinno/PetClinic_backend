using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;

namespace PetClinic.Api.Controllers.Admin;

/// <summary>
/// Admin appointments controller - manage appointments and scheduling
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/admin/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "Admin")]
public class AdminAppointmentsController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminAppointmentsController> _logger;

    public AdminAppointmentsController(IAdminService adminService, ILogger<AdminAppointmentsController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /admin/adminappointments - Display appointments list
    /// </summary>
    [HttpGet("index")]
    [HttpGet("")]
    public async Task<IActionResult> Index(DateTime? fromDate = null, DateTime? toDate = null, string? status = null)
    {
        _logger.LogInformation("Admin: Appointments list accessed (from: {FromDate}, to: {ToDate}, status: {Status})",
            fromDate, toDate, status);

        try
        {
            var appointments = await _adminService.GetAllAppointmentsAsync(fromDate, toDate, status);
            return View(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointments list");
            ModelState.AddModelError("Error", "Failed to load appointments");
            return View(new List<AdminAppointmentDto>());
        }
    }

    /// <summary>
    /// GET: /admin/adminappointments/details/id - Display appointment details
    /// </summary>
    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        _logger.LogInformation("Admin: Appointment details accessed for {AppointmentId}", id);

        try
        {
            var appointments = await _adminService.GetAllAppointmentsAsync();
            var appointment = appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment details for {AppointmentId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// GET: /admin/adminappointments/cancel/id - Display cancel confirmation
    /// </summary>
    [HttpGet("cancel/{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        _logger.LogInformation("Admin: Cancel appointment confirmation accessed for {AppointmentId}", id);

        try
        {
            var appointments = await _adminService.GetAllAppointmentsAsync();
            var appointment = appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cancel confirmation for {AppointmentId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// POST: /admin/adminappointments/cancel/id - Force cancel appointment
    /// </summary>
    [HttpPost("cancel/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(Guid id, [FromForm] string reason)
    {
        _logger.LogWarning("Admin: Force cancelling appointment {AppointmentId} - Reason: {Reason}", id, reason);

        try
        {
            await _adminService.CancelAppointmentAsync(id, reason);
            TempData["Success"] = "Appointment cancelled successfully";
            return RedirectToAction("Index");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Appointment not found: {AppointmentId}", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling appointment {AppointmentId}", id);
            ModelState.AddModelError("Error", "Failed to cancel appointment");
            return View("Cancel");
        }
    }

    /// <summary>
    /// GET: /admin/adminappointments/reassign/id - Display reassign form
    /// </summary>
    [HttpGet("reassign/{id:guid}")]
    public async Task<IActionResult> Reassign(Guid id)
    {
        _logger.LogInformation("Admin: Reassign appointment form accessed for {AppointmentId}", id);

        try
        {
            var appointments = await _adminService.GetAllAppointmentsAsync();
            var appointment = appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            var vets = await _adminService.GetAllVeterinariansAsync();
            ViewBag.Vets = vets;

            return View(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading reassign form for {AppointmentId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// POST: /admin/adminappointments/reassign/id - Reassign appointment to different vet
    /// </summary>
    [HttpPost("reassign/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReassignConfirmed(Guid id, [FromForm] Guid newVeterinarianId)
    {
        _logger.LogInformation("Admin: Reassigning appointment {AppointmentId} to veterinarian {NewVetId}", id, newVeterinarianId);

        try
        {
            await _adminService.ReassignAppointmentAsync(id, newVeterinarianId);
            TempData["Success"] = "Appointment reassigned successfully";
            return RedirectToAction("Details", new { id = id });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Appointment or veterinarian not found");
            ModelState.AddModelError("Error", ex.Message);
            return View("Reassign");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reassigning appointment {AppointmentId}", id);
            ModelState.AddModelError("Error", "Failed to reassign appointment");
            return View("Reassign");
        }
    }

    /// <summary>
    /// GET: /admin/adminappointments/api/list - Get all appointments (API)
    /// </summary>
    [HttpGet("api/list")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllAppointments(DateTime? fromDate = null, DateTime? toDate = null, string? status = null)
    {
        try
        {
            var appointments = await _adminService.GetAllAppointmentsAsync(fromDate, toDate, status);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments list");
            return BadRequest(new { error = ex.Message });
        }
    }
}
