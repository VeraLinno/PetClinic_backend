using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;

namespace PetClinic.Api.Controllers.Admin;

/// <summary>
/// Admin veterinarians controller - manage veterinary staff
/// </summary>
[ApiController]
[Route("admin/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "Admin")]
public class AdminVeterinariansController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminVeterinariansController> _logger;

    public AdminVeterinariansController(IAdminService adminService, ILogger<AdminVeterinariansController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /admin/adminveterinarians - Display veterinarians list
    /// </summary>
    [HttpGet("index")]
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Admin: Veterinarians list accessed");

        try
        {
            var vets = await _adminService.GetAllVeterinariansAsync();
            return View(vets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading veterinarians list");
            ModelState.AddModelError("Error", "Failed to load veterinarians");
            return View(new List<AdminVeterinarianDto>());
        }
    }

    /// <summary>
    /// GET: /admin/adminveterinarians/details/id - Display veterinarian details
    /// </summary>
    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        _logger.LogInformation("Admin: Veterinarian details accessed for {VetId}", id);

        try
        {
            var vet = await _adminService.GetVeterinarianByIdAsync(id);
            if (vet == null)
                return NotFound();

            return View(vet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading veterinarian details for {VetId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// POST: /admin/adminveterinarians/deactivate/id - Deactivate veterinarian
    /// </summary>
    [HttpPost("deactivate/{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        _logger.LogWarning("Admin: Deactivating veterinarian {VetId}", id);

        try
        {
            await _adminService.DeactivateVeterinarianAsync(id);
            TempData["Success"] = "Veterinarian deactivated successfully";
            return RedirectToAction("Details", new { id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating veterinarian {VetId}", id);
            ModelState.AddModelError("Error", "Failed to deactivate veterinarian");
            return RedirectToAction("Details", new { id = id });
        }
    }

    /// <summary>
    /// POST: /admin/adminveterinarians/reactivate/id - Reactivate veterinarian
    /// </summary>
    [HttpPost("reactivate/{id:guid}")]
    public async Task<IActionResult> Reactivate(Guid id)
    {
        _logger.LogInformation("Admin: Reactivating veterinarian {VetId}", id);

        try
        {
            await _adminService.ReactivateVeterinarianAsync(id);
            TempData["Success"] = "Veterinarian reactivated successfully";
            return RedirectToAction("Details", new { id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating veterinarian {VetId}", id);
            ModelState.AddModelError("Error", "Failed to reactivate veterinarian");
            return RedirectToAction("Details", new { id = id });
        }
    }

    /// <summary>
    /// GET: /admin/adminveterinarians/api/list - Get all veterinarians (API)
    /// </summary>
    [HttpGet("api/list")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllVeterinarians()
    {
        try
        {
            var vets = await _adminService.GetAllVeterinariansAsync();
            return Ok(vets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving veterinarians list");
            return BadRequest(new { error = ex.Message });
        }
    }
}
