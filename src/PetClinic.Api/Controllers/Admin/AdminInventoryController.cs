using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;

namespace PetClinic.Api.Controllers.Admin;

/// <summary>
/// Admin inventory controller - manage medication stock and inventory reports
/// </summary>
[ApiController]
[Route("admin/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "Admin")]
public class AdminInventoryController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminInventoryController> _logger;

    public AdminInventoryController(IAdminService adminService, ILogger<AdminInventoryController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /admin/admininventory - Display inventory report
    /// </summary>
    [HttpGet("index")]
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Admin: Inventory report page accessed");

        try
        {
            var inventory = await _adminService.GetInventoryReportAsync();
            return View(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading inventory report");
            ModelState.AddModelError("Error", "Failed to load inventory report");
            return View(new List<AdminInventoryReportDto>());
        }
    }

    /// <summary>
    /// GET: /admin/admininventory/lowstock - Display low stock medications
    /// </summary>
    [HttpGet("lowstock")]
    public async Task<IActionResult> LowStock()
    {
        _logger.LogInformation("Admin: Low stock medications page accessed");

        try
        {
            var lowStockMeds = await _adminService.GetLowStockMedicationsAsync();
            return View(lowStockMeds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading low stock medications");
            ModelState.AddModelError("Error", "Failed to load low stock medications");
            return View(new List<AdminInventoryReportDto>());
        }
    }

    /// <summary>
    /// GET: /admin/admininventory/medication/id - Display medication usage report
    /// </summary>
    [HttpGet("medication/{id:guid}")]
    public async Task<IActionResult> MedicationUsage(Guid id)
    {
        _logger.LogInformation("Admin: Medication usage report accessed for {MedicationId}", id);

        try
        {
            var usage = await _adminService.GetMedicationUsageAsync(id);
            return View(usage);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Medication not found: {MedicationId}", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading medication usage for {MedicationId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// GET: /admin/admininventory/api/report - Get inventory report (API)
    /// </summary>
    [HttpGet("api/report")]
    [Produces("application/json")]
    public async Task<IActionResult> GetInventoryReport()
    {
        try
        {
            var inventory = await _adminService.GetInventoryReportAsync();
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory report");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET: /admin/admininventory/api/lowstock - Get low stock medications (API)
    /// </summary>
    [HttpGet("api/lowstock")]
    [Produces("application/json")]
    public async Task<IActionResult> GetLowStockMedications()
    {
        try
        {
            var lowStockMeds = await _adminService.GetLowStockMedicationsAsync();
            return Ok(lowStockMeds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving low stock medications");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET: /admin/admininventory/api/medication/id - Get medication usage (API)
    /// </summary>
    [HttpGet("api/medication/{id:guid}")]
    [Produces("application/json")]
    public async Task<IActionResult> GetMedicationUsageApi(Guid id, int months = 3)
    {
        try
        {
            var usage = await _adminService.GetMedicationUsageAsync(id, months);
            return Ok(usage);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Medication not found: {MedicationId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medication usage");
            return BadRequest(new { error = ex.Message });
        }
    }
}
