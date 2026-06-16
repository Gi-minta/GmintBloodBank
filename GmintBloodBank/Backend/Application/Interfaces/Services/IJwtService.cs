namespace Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string username, string role, Guid? tenantId = null);
    string GenerateRefreshToken();
    (string Token, DateTime ExpiresAt) GenerateTokenPair(Guid userId, string username, string role, Guid? tenantId = null);
    Guid? ValidateRefreshToken(string token);
}
