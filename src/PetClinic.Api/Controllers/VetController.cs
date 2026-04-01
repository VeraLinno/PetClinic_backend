using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using PetClinic.Infrastructure;
using PetClinic.Domain;
using PetClinic.Application;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class VetController : ControllerBase
{
    private readonly PetClinicDbContext _context;
    private readonly IUserContextService _userContext;

    public VetController(PetClinicDbContext context, IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    /// <summary>
    /// Get all unavailable periods for the current veterinarian
    /// </summary>
    [HttpGet("unavailability")]
    public async Task<IActionResult> GetUnavailablePeriods()
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();

        if (!roles.Contains("Vet") && !roles.Contains("Admin"))
        {
            return Forbid();
        }

        var periods = await _context.VetUnavailabilities
            .Where(v => v.VeterinarianId == userId)
            .Select(v => new
            {
                v.Id,
                v.VeterinarianId,
                StartDate = v.StartDate.Date.ToString("yyyy-MM-dd"),
                EndDate = v.EndDate.Date.ToString("yyyy-MM-dd"),
                v.Reason
            })
            .ToListAsync();

        return Ok(periods);
    }

    /// <summary>
    /// Add an unavailable period for the current veterinarian
    /// </summary>
    [HttpPost("unavailability")]
    public async Task<IActionResult> AddUnavailablePeriod([FromBody] AddUnavailabilityDto dto)
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();

        if (!roles.Contains("Vet") && !roles.Contains("Admin"))
        {
            return Forbid();
        }

        // Validate dates
        if (string.IsNullOrEmpty(dto.StartDate) || string.IsNullOrEmpty(dto.EndDate))
        {
            return BadRequest(new { message = "Start and end dates are required" });
        }

        if (!DateTime.TryParse(dto.StartDate, out var startDate) ||
            !DateTime.TryParse(dto.EndDate, out var endDate))
        {
            return BadRequest(new { message = "Invalid date format" });
        }

        if (startDate > endDate)
        {
            return BadRequest(new { message = "Start date must be before or equal to end date" });
        }

        // Check for overlapping periods
        var hasOverlap = await _context.VetUnavailabilities
            .Where(v => v.VeterinarianId == userId)
            .AnyAsync(v =>
                startDate <= v.EndDate &&
                endDate >= v.StartDate);

        if (hasOverlap)
        {
            return BadRequest(new { message = "This period overlaps with an existing unavailable period" });
        }

        var unavailability = new VetUnavailability
        {
            VeterinarianId = userId,
            StartDate = startDate.Date,
            EndDate = endDate.Date,
            Reason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason.Trim()
        };

        _context.VetUnavailabilities.Add(unavailability);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            unavailability.Id,
            unavailability.VeterinarianId,
            StartDate = unavailability.StartDate.ToString("yyyy-MM-dd"),
            EndDate = unavailability.EndDate.ToString("yyyy-MM-dd"),
            unavailability.Reason
        });
    }

    /// <summary>
    /// Delete an unavailable period
    /// </summary>
    [HttpDelete("unavailability/{id:guid}")]
    public async Task<IActionResult> DeleteUnavailablePeriod(Guid id)
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();

        if (!roles.Contains("Vet") && !roles.Contains("Admin"))
        {
            return Forbid();
        }

        var unavailability = await _context.VetUnavailabilities
            .FirstOrDefaultAsync(v => v.Id == id && v.VeterinarianId == userId);

        if (unavailability == null)
        {
            return NotFound();
        }

        _context.VetUnavailabilities.Remove(unavailability);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Update an unavailable period
    /// </summary>
    [HttpPut("unavailability/{id:guid}")]
    public async Task<IActionResult> UpdateUnavailablePeriod(Guid id, [FromBody] AddUnavailabilityDto dto)
    {
        var userId = _userContext.GetCurrentUserId();
        var roles = _userContext.GetCurrentUserRoles();

        if (!roles.Contains("Vet") && !roles.Contains("Admin"))
        {
            return Forbid();
        }

        // Validate dates
        if (string.IsNullOrEmpty(dto.StartDate) || string.IsNullOrEmpty(dto.EndDate))
        {
            return BadRequest(new { message = "Start and end dates are required" });
        }

        if (!DateTime.TryParse(dto.StartDate, out var startDate) ||
            !DateTime.TryParse(dto.EndDate, out var endDate))
        {
            return BadRequest(new { message = "Invalid date format" });
        }

        if (startDate > endDate)
        {
            return BadRequest(new { message = "Start date must be before or equal to end date" });
        }

        var unavailability = await _context.VetUnavailabilities
            .FirstOrDefaultAsync(v => v.Id == id && v.VeterinarianId == userId);

        if (unavailability == null)
        {
            return NotFound();
        }

        // Check for overlapping periods (excluding the current one)
        var hasOverlap = await _context.VetUnavailabilities
            .Where(v => v.VeterinarianId == userId && v.Id != id)
            .AnyAsync(v =>
                startDate <= v.EndDate &&
                endDate >= v.StartDate);

        if (hasOverlap)
        {
            return BadRequest(new { message = "This period overlaps with an existing unavailable period" });
        }

        unavailability.StartDate = startDate.Date;
        unavailability.EndDate = endDate.Date;
        unavailability.Reason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason.Trim();

        await _context.SaveChangesAsync();

        return Ok(new
        {
            unavailability.Id,
            unavailability.VeterinarianId,
            StartDate = unavailability.StartDate.ToString("yyyy-MM-dd"),
            EndDate = unavailability.EndDate.ToString("yyyy-MM-dd"),
            unavailability.Reason
        });
    }
}

public class AddUnavailabilityDto
{
    public string StartDate { get; set; } = default!;
    public string EndDate { get; set; } = default!;
    public string? Reason { get; set; }
}
