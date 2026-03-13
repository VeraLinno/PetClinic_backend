using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PetClinic.Application;
using PetClinic.Domain;

namespace PetClinic.Infrastructure;

public class AuthService : IAuthService
{
    private readonly PetClinicDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(PetClinicDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Owners.AnyAsync(o => o.Email == request.Email))
        {
            return new AuthResult { Success = false, Error = "Email already exists" };
        }

        var owner = new Owner
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Roles = request.Roles
        };

        _context.Owners.Add(owner);
        await _context.SaveChangesAsync();

        return new AuthResult { Success = true };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Email == request.Email);
        if (owner == null || !BCrypt.Net.BCrypt.Verify(request.Password, owner.PasswordHash))
        {
            return new AuthResult { Success = false, Error = "Invalid credentials" };
        }

        var accessToken = GenerateAccessToken(owner);
        var refreshToken = await GenerateRefreshTokenAsync(owner);

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
            return new AuthResult { Success = false, Error = "Invalid or expired refresh token" };
        }

        // Rotate token
        var newRefreshToken = await GenerateRefreshTokenAsync(storedToken.Owner);
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = HashRefreshToken(newRefreshToken);
        await _context.SaveChangesAsync();

        var accessToken = GenerateAccessToken(storedToken.Owner);

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

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
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
}