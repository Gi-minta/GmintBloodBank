using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public sealed class JwtTokenService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(Guid userId, string username, string role, Guid? tenantId = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSuperSecretKeyForDevelopment123!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role),
        };

        if (tenantId.HasValue)
            claims.Add(new("tenant_id", tenantId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "GmintBloodBank",
            audience: _configuration["Jwt:Audience"] ?? "GmintBloodBank",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public (string Token, DateTime ExpiresAt) GenerateTokenPair(Guid userId, string username, string role, Guid? tenantId = null)
    {
        var accessToken = GenerateAccessToken(userId, username, role, tenantId);
        var expiresAt = DateTime.UtcNow.AddHours(1);
        return (accessToken, expiresAt);
    }

    public Guid? ValidateRefreshToken(string token)
    {
        // TODO: Implement actual refresh token validation against DB
        return null;
    }
}
