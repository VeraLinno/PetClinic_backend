using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using PetClinic.Application;

namespace PetClinic.Api.Controllers.Admin;

/// <summary>
/// Admin users controller - manage system users and assignments.
/// Only accessible to users with Admin role.
/// </summary>
[ApiController]
[Route("admin/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = "Admin")]
public class AdminUsersController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminUsersController> _logger;

    public AdminUsersController(IAdminService adminService, ILogger<AdminUsersController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// GET: /admin/adminusers - Display users list
    /// </summary>
    [HttpGet("index")]
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Admin: Users list accessed");

        try
        {
            var users = await _adminService.GetAllUsersAsync();
            return View(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users list");
            ModelState.AddModelError("Error", "Failed to load users");
            return View(new List<AdminUserDto>());
        }
    }

    /// <summary>
    /// GET: /admin/adminusers/details/id - Display user details and activity
    /// </summary>
    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        _logger.LogInformation("Admin: User details accessed for {UserId}", id);

        try
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var activity = await _adminService.GetUserActivityAsync(id);

            return View(new { User = user, Activity = activity });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user details for {UserId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// GET: /admin/adminusers/edit/id - Display edit user form
    /// </summary>
    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        _logger.LogInformation("Admin: Edit user form accessed for {UserId}", id);

        try
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user edit form for {UserId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// POST: /admin/adminusers/edit/id - Update user roles and status
    /// </summary>
    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] UpdateUserRoleDto dto)
    {
        _logger.LogInformation("Admin: Updating user {UserId}", id);

        try
        {
            var updatedUser = await _adminService.UpdateUserAsync(id, dto);
            TempData["Success"] = "User updated successfully";
            return RedirectToAction("Details", new { id = id });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found: {UserId}", id);
            ModelState.AddModelError("Error", ex.Message);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            ModelState.AddModelError("Error", "Failed to update user");
            return View();
        }
    }

    /// <summary>
    /// GET: /admin/adminusers/delete/id - Display delete confirmation
    /// </summary>
    [HttpGet("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Admin: Delete user confirmation accessed for {UserId}", id);

        try
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation for {UserId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// POST: /admin/adminusers/delete/id - Confirm delete user
    /// </summary>
    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        _logger.LogWarning("Admin: Deleting user {UserId}", id);

        try
        {
            await _adminService.DeleteUserAsync(id);
            TempData["Success"] = "User deleted successfully";
            return RedirectToAction("Index");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found: {UserId}", id);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            ModelState.AddModelError("Error", "Failed to delete user");
            return View();
        }
    }

    /// <summary>
    /// GET: /admin/adminusers/api/list - Get all users (API endpoint)
    /// </summary>
    [HttpGet("api/list")]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users list");
            return BadRequest(new { error = ex.Message });
        }
    }
}
