using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PetClinic.Application;
using PetClinic.Domain;

namespace PetClinic.Infrastructure;

public class AuthService : IAuthService
{
    private readonly PetClinicDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(PetClinicDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        if (string.IsNullOrEmpty(normalizedEmail))
        {
            return new AuthResult { Success = false, Error = "Email is required" };
        }

        var requestedRoles = NormalizeRoles(request.Roles);
        if (requestedRoles.Any(role => role.Equals("Vet", StringComparison.OrdinalIgnoreCase)))
        {
            return new AuthResult { Success = false, Error = "Vet accounts can only be created by existing veterinarians" };
        }

        if (await _context.Owners.AnyAsync(o => o.Email.ToLower() == normalizedEmail))
        {
            return new AuthResult { Success = false, Error = "Email already exists" };
        }

        var owner = new Owner
        {
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Roles = new List<string> { "Owner" }
        };

        _context.Owners.Add(owner);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {Email}", normalizedEmail);
        return new AuthResult { Success = true };
    }

    public async Task<AuthResult> CreateVetAccountAsync(CreateVetAccountRequest request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        if (string.IsNullOrEmpty(normalizedEmail))
        {
            return new AuthResult { Success = false, Error = "Email is required" };
        }

        var firstName = request.FirstName?.Trim() ?? string.Empty;
        var lastName = request.LastName?.Trim() ?? string.Empty;
        var licenseNumber = request.LicenseNumber?.Trim() ?? string.Empty;
        if (firstName.Length == 0 || lastName.Length == 0)
        {
            return new AuthResult { Success = false, Error = "First and last name are required" };
        }

        if (licenseNumber.Length == 0)
        {
            return new AuthResult { Success = false, Error = "License number is required" };
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return new AuthResult { Success = false, Error = "Password must be at least 8 characters" };
        }

        var phoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        if (!string.IsNullOrEmpty(phoneNumber) && !Regex.IsMatch(phoneNumber, @"^[+0-9()\-\s]{7,20}$"))
        {
            return new AuthResult { Success = false, Error = "Phone number format is invalid" };
        }

        var ownerEmailExists = await _context.Owners.AnyAsync(o => o.Email.ToLower() == normalizedEmail);
        if (ownerEmailExists)
        {
            return new AuthResult { Success = false, Error = "Email already exists" };
        }

        var vetEmailExists = await _context.Veterinarians.AnyAsync(v => v.Email.ToLower() == normalizedEmail);
        if (vetEmailExists)
        {
            return new AuthResult { Success = false, Error = "Email already exists" };
        }

        var licenseExists = await _context.Veterinarians.AnyAsync(v => v.LicenseNumber.ToLower() == licenseNumber.ToLower());
        if (licenseExists)
        {
            return new AuthResult { Success = false, Error = "License number already exists" };
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var vetOwner = new Owner
            {
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = firstName,
                LastName = lastName,
                Roles = new List<string> { "Vet" }
            };

            _context.Owners.Add(vetOwner);

            var vetProfile = new Veterinarian
            {
                Id = vetOwner.Id,
                Name = firstName,
                LastName = lastName,
                Email = normalizedEmail,
                PhoneNumber = phoneNumber,
                LicenseNumber = licenseNumber
            };

            _context.Veterinarians.Add(vetProfile);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new AuthResult { Success = true };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        if (string.IsNullOrEmpty(normalizedEmail))
        {
            _logger.LogWarning("Login attempted with invalid email format");
            return new AuthResult { Success = false, Error = "Invalid credentials" };
        }

        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Email.ToLower() == normalizedEmail);
        if (owner == null || !BCrypt.Net.BCrypt.Verify(request.Password, owner.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", normalizedEmail);
            return new AuthResult { Success = false, Error = "Invalid credentials" };
        }

        var accessToken = GenerateAccessToken(owner);
        var refreshToken = await GenerateRefreshTokenAsync(owner);

        _logger.LogInformation("User logged in successfully: {Email} ({UserId})", normalizedEmail, owner.Id);
        return new AuthResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var tokenHash = HashRefreshToken(refreshToken);
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.Owner)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && !rt.RevokedAt.HasValue);

        if (storedToken == null || storedToken.Expires < DateTime.UtcNow)
        {
            _logger.LogWarning("Failed token refresh: Invalid or expired token");
            return new AuthResult { Success = false, Error = "Invalid or expired refresh token" };
        }

        // Rotate token
        var newRefreshToken = await GenerateRefreshTokenAsync(storedToken.Owner);
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = HashRefreshToken(newRefreshToken);
        await _context.SaveChangesAsync();

        var accessToken = GenerateAccessToken(storedToken.Owner);

        _logger.LogInformation("Token refreshed successfully for user: {UserId}", storedToken.Owner.Id);
        return new AuthResult
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenHash = HashRefreshToken(refreshToken);
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken != null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("User logged out successfully: {UserId}", storedToken.OwnerId);
        }
    }

    private string GenerateAccessToken(Owner owner)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, owner.Id.ToString()),
            new Claim("roles", string.Join(",", owner.Roles))
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Security: 5-minute expiration balances security (short window for token theft) with UX
        // (auto-refresh handles seamlessly). Industry standard: OIDC/OAuth 2.0 recommend 5-10 min.
        // If token is intercepted (XSS, MITM), attacker has only 5-min window vs 15-min (3x reduction).
        // Frontend auto-refresh ensures no UX impact - users don't notice the shorter token lifetime.
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(Owner owner)
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        var refreshToken = Convert.ToBase64String(randomBytes);

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = HashRefreshToken(refreshToken),
            Expires = DateTime.UtcNow.AddDays(7),
            OwnerId = owner.Id
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    private string HashRefreshToken(string token)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    private static string NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email) ? string.Empty : email.Trim().ToLowerInvariant();
    }

    private static List<string> NormalizeRoles(IEnumerable<string>? roles)
    {
        if (roles == null)
        {
            return new List<string>();
        }

        return roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}