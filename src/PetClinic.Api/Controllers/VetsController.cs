using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using PetClinic.Application;
using PetClinic.Infrastructure;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class VetsController : ControllerBase
{
    private const string MainVetEmail = "vet@petclinic.com";

    private readonly PetClinicDbContext _context;
    private readonly IUserContextService _userContext;
    private readonly IAuthService _authService;

    public VetsController(PetClinicDbContext context, IUserContextService userContext, IAuthService authService)
    {
        _context = context;
        _userContext = userContext;
        _authService = authService;
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetVetAccounts()
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        var accounts = await _context.Veterinarians
            .Select(v => new VetAccountDto
            {
                Id = v.Id,
                Email = v.Email,
                FirstName = v.Name,
                LastName = v.LastName,
                LicenseNumber = v.LicenseNumber,
                PhoneNumber = v.PhoneNumber
            })
            .OrderBy(v => v.LastName)
            .ThenBy(v => v.FirstName)
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpPost("accounts")]
    public async Task<IActionResult> CreateVetAccount([FromBody] CreateVetAccountRequest request)
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        var result = await _authService.CreateVetAccountAsync(request);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok();
    }

    [HttpPut("accounts/{id:guid}")]
    public async Task<IActionResult> UpdateVetAccount(Guid id, [FromBody] UpdateVetAccountDto dto)
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == id);
        var vetProfile = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == id);

        if (owner == null || vetProfile == null || !owner.Roles.Contains("Vet"))
        {
            return NotFound();
        }

        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var normalizedFirstName = dto.FirstName.Trim();
        var normalizedLastName = dto.LastName.Trim();
        var normalizedLicense = dto.LicenseNumber.Trim();
        var normalizedPhone = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();

        var ownerEmailInUse = await _context.Owners
            .AnyAsync(o => o.Id != id && o.Email.ToLower() == normalizedEmail);
        if (ownerEmailInUse)
        {
            return Conflict(new { error = "Email is already in use" });
        }

        var vetEmailInUse = await _context.Veterinarians
            .AnyAsync(v => v.Id != id && v.Email.ToLower() == normalizedEmail);
        if (vetEmailInUse)
        {
            return Conflict(new { error = "Email is already in use" });
        }

        var licenseInUse = await _context.Veterinarians
            .AnyAsync(v => v.Id != id && v.LicenseNumber.ToLower() == normalizedLicense.ToLower());
        if (licenseInUse)
        {
            return Conflict(new { error = "License number is already in use" });
        }

        owner.Email = normalizedEmail;
        owner.FirstName = normalizedFirstName;
        owner.LastName = normalizedLastName;

        vetProfile.Email = normalizedEmail;
        vetProfile.Name = normalizedFirstName;
        vetProfile.LastName = normalizedLastName;
        vetProfile.LicenseNumber = normalizedLicense;
        vetProfile.PhoneNumber = normalizedPhone;

        await _context.SaveChangesAsync();

        return Ok(new VetAccountDto
        {
            Id = vetProfile.Id,
            Email = vetProfile.Email,
            FirstName = vetProfile.Name,
            LastName = vetProfile.LastName,
            LicenseNumber = vetProfile.LicenseNumber,
            PhoneNumber = vetProfile.PhoneNumber
        });
    }

    [HttpDelete("accounts/{id:guid}")]
    public async Task<IActionResult> DeleteVetAccount(Guid id)
    {
        if (!IsCurrentUserVet())
        {
            return Forbid();
        }

        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == id);
        var vetProfile = await _context.Veterinarians.FirstOrDefaultAsync(v => v.Id == id);

        if (owner == null || vetProfile == null || !owner.Roles.Contains("Vet"))
        {
            return NotFound();
        }

        var currentUserId = _userContext.GetCurrentUserId();
        var isMainVet = await IsCurrentUserMainVetAsync();
        var canDelete = isMainVet || currentUserId == id;

        if (!canDelete)
        {
            return Forbid();
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var appointments = await _context.Appointments
                .Where(a => a.VeterinarianId == id)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                appointment.VeterinarianId = null;
            }

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.OwnerId == id)
                .ToListAsync();

            if (refreshTokens.Count > 0)
            {
                _context.RefreshTokens.RemoveRange(refreshTokens);
            }

            _context.Veterinarians.Remove(vetProfile);
            _context.Owners.Remove(owner);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private bool IsCurrentUserVet()
    {
        var roles = _userContext.GetCurrentUserRoles();
        return roles.Contains("Vet");
    }

    private async Task<bool> IsCurrentUserMainVetAsync()
    {
        var currentUserId = _userContext.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return false;
        }

        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == currentUserId);
        if (owner == null)
        {
            return false;
        }

        return string.Equals(owner.Email, MainVetEmail, StringComparison.OrdinalIgnoreCase);
    }
}
