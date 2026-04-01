using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;
using PetClinic.Application;

namespace PetClinic.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserContextService _userContext;

    public AuthController(IAuthService authService, IUserContextService userContext)
    {
        _authService = authService;
        _userContext = userContext;
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }
        return Ok();
    }

    [Authorize]
    [HttpPost("register-vet")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> RegisterVet([FromBody] CreateVetAccountRequest request)
    {
        var roles = _userContext.GetCurrentUserRoles();
        if (!roles.Contains("Admin"))
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

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result.MfaRequired)
        {
            return Ok(new { mfaRequired = true, message = result.Error });
        }

        if (!result.Success)
        {
            return Unauthorized(new { error = result.Error });
        }

        Response.Cookies.Append("refreshToken", result.RefreshToken!, BuildRefreshCookieOptions());

        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { error = "Refresh token missing" });
        }

        var result = await _authService.RefreshTokenAsync(refreshToken);
        if (!result.Success)
        {
            return Unauthorized(new { error = result.Error });
        }

        Response.Cookies.Append("refreshToken", result.RefreshToken!, BuildRefreshCookieOptions());

        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("logout")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.LogoutAsync(refreshToken);
        }

        Response.Cookies.Delete("refreshToken", BuildRefreshDeleteCookieOptions());
        return NoContent();
    }

    private CookieOptions BuildRefreshCookieOptions()
    {
        var isHttps = Request.IsHttps;

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"
        };
    }

    private CookieOptions BuildRefreshDeleteCookieOptions()
    {
        var isHttps = Request.IsHttps;

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/"
        };
    }
}