using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;
using PetClinic.Api.ViewModels;

namespace PetClinic.Api.Controllers.Admin;

/// <summary>
/// Admin dashboard controller - main entry point for admin console.
/// Only accessible to users with Admin role.
/// </summary>
[ApiController]
[Area("Admin")]
[Route("admin/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "Admin")]
public class AdminDashboardController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminDashboardController> _logger;

    public AdminDashboardController(IAdminService adminService, ILogger<AdminDashboardController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /admin/admindashboard/index - Display admin dashboard overview
    /// </summary>
    [HttpGet("index")]
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Admin Dashboard accessed");

        try
        {
            var metrics = await _adminService.GetDashboardMetricsAsync();
            var health = await _adminService.GetSystemHealthAsync();
            var model = new AdminDashboardPageViewModel
            {
                Metrics = metrics,
                Health = health
            };

            return View("~/Views/Admin/Dashboard/Index.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            ModelState.AddModelError("Error", "Failed to load dashboard metrics");
            return View("~/Views/Admin/Dashboard/Index.cshtml", new AdminDashboardPageViewModel());
        }
    }

    /// <summary>
    /// GET: /admin/admindashboard/metrics - Get dashboard metrics (API endpoint)
    /// </summary>
    [HttpGet("metrics")]
    [Produces("application/json")]
    public async Task<IActionResult> GetMetrics()
    {
        try
        {
            var metrics = await _adminService.GetDashboardMetricsAsync();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard metrics");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET: /admin/admindashboard/health - Get system health status
    /// </summary>
    [HttpGet("health")]
    [Produces("application/json")]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var health = await _adminService.GetSystemHealthAsync();
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system health");
            return BadRequest(new { error = ex.Message });
        }
    }
}
