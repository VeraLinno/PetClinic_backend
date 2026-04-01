using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;

namespace PetClinic.Api.Controllers.Admin;

/// <summary>
/// Admin cleanup controller - preview vet account cleanup candidates (dry-run mode)
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/admin/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "Admin")]
public class CleanupController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<CleanupController> _logger;

    public CleanupController(IAdminService adminService, ILogger<CleanupController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// POST: /api/admin/cleanup/vet-accounts/dry-run - Preview vet accounts eligible for cleanup.
    /// This endpoint performs no deletions; it only previews candidates for review.
    /// </summary>
    [HttpPost("vet-accounts/dry-run")]
    [Produces("application/json")]
    public async Task<IActionResult> PreviewVetAccountCleanup()
    {
        _logger.LogInformation("Admin: Vet account cleanup dry-run preview requested");

        try
        {
            var result = await _adminService.PreviewVetAccountCleanupAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing vet account cleanup");
            return BadRequest(new { error = "Failed to preview vet account cleanup", details = ex.Message });
        }
    }
}
