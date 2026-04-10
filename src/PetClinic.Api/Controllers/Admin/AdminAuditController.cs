using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;
using PetClinic.Api.ViewModels;

namespace PetClinic.Api.Controllers.Admin;

/// <summary>
/// Admin audit controller - view audit logs and user activity records
/// </summary>
[ApiController]
[Area("Admin")]
[Route("admin/[controller]")]
[Route("api/v{version:apiVersion}/admin/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "Admin")]
public class AdminAuditController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminAuditController> _logger;

    public AdminAuditController(IAdminService adminService, ILogger<AdminAuditController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /admin/adminaudit - Display audit logs list
    /// </summary>
    [HttpGet("index")]
    [HttpGet("")]
    public async Task<IActionResult> Index(int days = 30, string? userEmail = null, string? action = null)
    {
        _logger.LogInformation("Admin: Audit logs page accessed (days: {Days}, email: {Email}, action: {Action})",
            days, userEmail, action);

        try
        {
            var logs = await _adminService.GetAuditLogsAsync(days, userEmail, action);
            return View("~/Views/Admin/Audit/Index.cshtml", new AdminAuditIndexPageViewModel
            {
                Logs = logs,
                Days = days,
                UserEmail = userEmail,
                Action = action
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading audit logs");
            ModelState.AddModelError("Error", "Failed to load audit logs");
            return View("~/Views/Admin/Audit/Index.cshtml", new AdminAuditIndexPageViewModel
            {
                Days = days,
                UserEmail = userEmail,
                Action = action
            });
        }
    }

    /// <summary>
    /// GET: /admin/adminaudit/search - Display audit log search form
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search(AdminAuditLogFilterDto? filter = null)
    {
        _logger.LogInformation("Admin: Audit log search form accessed");

        try
        {
            if (filter == null)
            {
                return View("~/Views/Admin/Audit/Search.cshtml", new AdminAuditSearchPageViewModel());
            }

            var logs = await _adminService.SearchAuditLogsAsync(filter);
            return View("~/Views/Admin/Audit/Search.cshtml", new AdminAuditSearchPageViewModel
            {
                Filter = filter,
                Results = logs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching audit logs");
            ModelState.AddModelError("Error", "Failed to search audit logs");
            return View("~/Views/Admin/Audit/Search.cshtml", new AdminAuditSearchPageViewModel
            {
                Filter = filter ?? new AdminAuditLogFilterDto()
            });
        }
    }

    /// <summary>
    /// POST: /admin/adminaudit/search - Process audit log search
    /// </summary>
    [HttpPost("search")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchResults([FromForm] AdminAuditLogFilterDto filter)
    {
        _logger.LogInformation("Admin: Audit log search executed with filter");

        try
        {
            var logs = await _adminService.SearchAuditLogsAsync(filter);
            return View("~/Views/Admin/Audit/Search.cshtml", new AdminAuditSearchPageViewModel
            {
                Filter = filter,
                Results = logs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching audit logs");
            ModelState.AddModelError("Error", "Failed to search audit logs");
            return View("~/Views/Admin/Audit/Search.cshtml", new AdminAuditSearchPageViewModel
            {
                Filter = filter
            });
        }
    }

    /// <summary>
    /// GET: /admin/adminaudit/user/userId - Display user activity timeline
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> UserActivity(Guid userId, int days = 30)
    {
        _logger.LogInformation("Admin: User activity timeline accessed for {UserId} (last {Days} days)", userId, days);

        try
        {
            var activity = await _adminService.GetUserActivityAsync(userId, days);
            return View("~/Views/Admin/Audit/UserActivity.cshtml", new AdminAuditUserActivityPageViewModel
            {
                UserId = userId,
                Days = days,
                Activities = activity
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user activity for {UserId}", userId);
            ModelState.AddModelError("Error", "Failed to load user activity");
            return View("~/Views/Admin/Audit/UserActivity.cshtml", new AdminAuditUserActivityPageViewModel
            {
                UserId = userId,
                Days = days
            });
        }
    }

    /// <summary>
    /// GET: /admin/adminaudit/api/logs - Get audit logs (API)
    /// </summary>
    [HttpGet("api/logs")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAuditLogs(int days = 30, string? userEmail = null, string? action = null)
    {
        try
        {
            var logs = await _adminService.GetAuditLogsAsync(days, userEmail, action);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// POST: /admin/adminaudit/api/search - Search audit logs (API)
    /// </summary>
    [HttpPost("api/search")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> SearchAuditLogs([FromBody] AdminAuditLogFilterDto filter)
    {
        try
        {
            var logs = await _adminService.SearchAuditLogsAsync(filter);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching audit logs");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// GET: /admin/adminaudit/api/user/userId - Get user activity (API)
    /// </summary>
    [HttpGet("api/user/{userId:guid}")]
    [Produces("application/json")]
    public async Task<IActionResult> GetUserActivityApi(Guid userId, int days = 30)
    {
        try
        {
            var activity = await _adminService.GetUserActivityAsync(userId, days);
            return Ok(activity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user activity");
            return BadRequest(new { error = ex.Message });
        }
    }
}
