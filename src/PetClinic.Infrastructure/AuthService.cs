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
using System.Globalization;

namespace PetClinic.Infrastructure;

public class AuthService : IAuthService
{
    private readonly PetClinicDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IUserContextService? _userContext;
    private const int TotpTimeStepSeconds = 30;
    private const int TotpDigits = 6;

    public AuthService(PetClinicDbContext context, IConfiguration configuration, ILogger<AuthService> logger, IUserContextService? userContext = null)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed unexpectedly for email: {Email}", request.Email);
            return new AuthResult { Success = false, Error = "Registration failed" };
        }
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
            // Get user context if available (optional)
            var currentUserId = _userContext?.GetCurrentUserId() ?? Guid.Empty;
            var currentUserRoles = _userContext?.GetCurrentUserRoles() ?? new List<string>();
            var rolesCsv = string.Join(",", currentUserRoles);

            var vetOwner = new Owner
            {
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = firstName,
                LastName = lastName,
                Roles = new List<string> { "Vet" },
                // Capture creation metadata (only if user context is available)
                VetAccountCreatedAtUtc = _userContext != null ? DateTime.UtcNow : null,
                VetAccountCreatedByUserId = _userContext != null && currentUserId != Guid.Empty ? currentUserId : null,
                VetAccountCreatedByRolesCsv = _userContext != null && !string.IsNullOrEmpty(rolesCsv) ? rolesCsv : null
            };

            _context.Owners.Add(vetOwner);

            var vetProfile = new Veterinarian
            {
                Id = vetOwner.Id,
                Name = firstName,
                LastName = lastName,
                Email = normalizedEmail,
                PhoneNumber = phoneNumber,
                LicenseNumber = licenseNumber,
                // Capture creation metadata (only if user context is available)
                VetAccountCreatedAtUtc = _userContext != null ? DateTime.UtcNow : null,
                VetAccountCreatedByUserId = _userContext != null && currentUserId != Guid.Empty ? currentUserId : null,
                VetAccountCreatedByRolesCsv = _userContext != null && !string.IsNullOrEmpty(rolesCsv) ? rolesCsv : null
            };

            _context.Veterinarians.Add(vetProfile);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            if (_userContext != null)
            {
                _logger.LogInformation("Vet account created: {Email} by user {CreatedByUserId} with roles {Roles}",
                    normalizedEmail, currentUserId, rolesCsv);
            }
            else
            {
                _logger.LogInformation("Vet account created: {Email} (no user context available)", normalizedEmail);
            }

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
        try
        {
            var normalizedEmail = NormalizeEmail(request.Email);
            if (string.IsNullOrEmpty(normalizedEmail))
            {
                _logger.LogWarning("Login attempted with invalid email format");
                return new AuthResult { Success = false, Error = "Invalid credentials" };
            }

            var ownerData = await _context.Owners
                .AsNoTracking()
                .Where(o => o.Email.ToLower() == normalizedEmail)
                .Select(o => new
                {
                    o.Id,
                    o.Email,
                    o.PasswordHash,
                    o.Roles
                })
                .FirstOrDefaultAsync();

            if (ownerData == null)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", normalizedEmail);
                return new AuthResult { Success = false, Error = "Invalid credentials" };
            }

            var owner = new Owner
            {
                Id = ownerData.Id,
                Email = ownerData.Email,
                PasswordHash = ownerData.PasswordHash,
                Roles = ownerData.Roles ?? new List<string>()
            };

            var passwordHash = owner.PasswordHash ?? string.Empty;
            var incomingPassword = request.Password ?? string.Empty;
            var isValidPassword = VerifyPassword(incomingPassword, passwordHash, out var shouldRehashPassword);

            if (!isValidPassword)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", normalizedEmail);
                return new AuthResult { Success = false, Error = "Invalid credentials" };
            }

            if (shouldRehashPassword)
            {
                try
                {
                    var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(incomingPassword);
                    await _context.Owners
                        .Where(o => o.Id == owner.Id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(o => o.PasswordHash, newPasswordHash));
                    _logger.LogInformation("Upgraded legacy password hash for user {UserId}", owner.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to upgrade legacy password hash for user {UserId}", owner.Id);
                }
            }

            var configuredAdminEmail = NormalizeEmail(_configuration["AdminAccess:AdminEmail"]);
            var ownerRoles = owner.Roles ?? new List<string>();
            var isAdminUser = ownerRoles.Contains("Admin", StringComparer.OrdinalIgnoreCase);
            var isConfiguredAdmin = !string.IsNullOrWhiteSpace(configuredAdminEmail)
                && normalizedEmail.Equals(configuredAdminEmail, StringComparison.OrdinalIgnoreCase);

            if (isAdminUser && !isConfiguredAdmin)
            {
                _logger.LogWarning("Blocked admin login for non-allowlisted account: {Email}", normalizedEmail);
                return new AuthResult { Success = false, Error = "This account is not allowed to access admin features" };
            }

            var requireAdminMfa = _configuration.GetValue<bool?>("AdminAccess:RequireMfa") ?? true;
            if (isConfiguredAdmin && isAdminUser && requireAdminMfa)
            {
                var mfaSecret = _configuration["AdminAccess:MfaSecret"]?.Trim();
                if (string.IsNullOrWhiteSpace(mfaSecret))
                {
                    _logger.LogError("Admin MFA is enabled but AdminAccess:MfaSecret is not configured");
                    return new AuthResult { Success = false, Error = "Admin MFA is not configured on the server" };
                }

                if (string.IsNullOrWhiteSpace(request.MfaCode))
                {
                    return new AuthResult { Success = false, MfaRequired = true, Error = "MFA code is required" };
                }

                if (!ValidateTotpCode(mfaSecret, request.MfaCode))
                {
                    _logger.LogWarning("Invalid MFA code for admin login: {Email}", normalizedEmail);
                    return new AuthResult { Success = false, Error = "Invalid MFA code" };
                }
            }

            var accessToken = GenerateAccessToken(owner);
            string? refreshToken = null;
            try
            {
                refreshToken = await GenerateRefreshTokenAsync(owner);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist refresh token for user {UserId}", owner.Id);
            }

            _logger.LogInformation("User logged in successfully: {Email} ({UserId})", normalizedEmail, owner.Id);
            return new AuthResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed unexpectedly for email: {Email}", request.Email);
            return new AuthResult { Success = false, Error = "Login failed" };
        }
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
        var normalizedEmail = NormalizeEmail(owner.Email);
        var configuredAdminEmail = NormalizeEmail(_configuration["AdminAccess:AdminEmail"]);
        var allowedRoles = (owner.Roles ?? new List<string>())
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Where(role => !role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(configuredAdminEmail)
                    && normalizedEmail.Equals(configuredAdminEmail, StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, owner.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, owner.Email),
            new Claim("roles", string.Join(",", allowedRoles))
        };

        foreach (var role in allowedRoles)
        {
            claims.Add(new Claim("roles", role));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

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

    private bool VerifyPassword(string incomingPassword, string storedPasswordHash, out bool shouldRehashPassword)
    {
        shouldRehashPassword = false;
        if (string.IsNullOrEmpty(incomingPassword) || string.IsNullOrWhiteSpace(storedPasswordHash))
        {
            return false;
        }

        try
        {
            if (BCrypt.Net.BCrypt.Verify(incomingPassword, storedPasswordHash))
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Password hash verification failed, falling back to legacy check");
        }

        if (string.Equals(incomingPassword, storedPasswordHash, StringComparison.Ordinal))
        {
            shouldRehashPassword = true;
            _logger.LogInformation("Accepted legacy plain-text password hash for user login; hash will be upgraded on successful login");
            return true;
        }

        return false;
    }

    private static bool ValidateTotpCode(string base32Secret, string mfaCode)
    {
        if (string.IsNullOrWhiteSpace(mfaCode))
        {
            return false;
        }

        var normalizedCode = new string(mfaCode.Where(char.IsDigit).ToArray());
        if (normalizedCode.Length != TotpDigits)
        {
            return false;
        }

        var secretBytes = DecodeBase32(base32Secret);
        if (secretBytes.Length == 0)
        {
            return false;
        }

        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var timestep = unixTime / TotpTimeStepSeconds;

        // Accept a small clock skew window of +-1 step.
        for (var offset = -1; offset <= 1; offset++)
        {
            var expected = ComputeTotp(secretBytes, timestep + offset);
            if (CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected),
                Encoding.UTF8.GetBytes(normalizedCode)))
            {
                return true;
            }
        }

        return false;
    }

    private static string ComputeTotp(byte[] key, long timestep)
    {
        var timestepBytes = BitConverter.GetBytes(timestep);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timestepBytes);
        }

        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(timestepBytes);

        var offset = hash[^1] & 0x0F;
        var binaryCode = ((hash[offset] & 0x7F) << 24)
            | (hash[offset + 1] << 16)
            | (hash[offset + 2] << 8)
            | hash[offset + 3];

        var otp = binaryCode % (int)Math.Pow(10, TotpDigits);
        return otp.ToString(CultureInfo.InvariantCulture).PadLeft(TotpDigits, '0');
    }

    private static byte[] DecodeBase32(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Array.Empty<byte>();
        }

        var normalized = input.Trim().Replace("=", string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        var output = new List<byte>();
        var buffer = 0;
        var bitsLeft = 0;

        foreach (var c in normalized)
        {
            var val = alphabet.IndexOf(c);
            if (val < 0)
            {
                return Array.Empty<byte>();
            }

            buffer = (buffer << 5) | val;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                bitsLeft -= 8;
                output.Add((byte)((buffer >> bitsLeft) & 0xFF));
            }
        }

        return output.ToArray();
    }
}