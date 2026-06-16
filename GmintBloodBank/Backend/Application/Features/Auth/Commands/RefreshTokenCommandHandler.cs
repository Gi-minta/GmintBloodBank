using Application.Common.CQRS;
using Application.DTOs.Auth;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities.Security;

namespace Application.Features.Auth.Commands;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponseDto>
{
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IDateTimeProvider _dateTime;

    public RefreshTokenCommandHandler(
        IRepository<RefreshToken> refreshTokenRepository,
        IRepository<User> userRepository,
        IJwtService jwtService,
        IDateTimeProvider dateTime)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _dateTime = dateTime;
    }

    public async Task<LoginResponseDto> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var allTokens = await _refreshTokenRepository.GetAllAsync(cancellationToken);
        var storedToken = allTokens.FirstOrDefault(rt =>
            rt.Token == command.RefreshToken && rt.IsActive);

        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        if (storedToken.UserId is null)
            throw new UnauthorizedAccessException("Corrupted refresh token — no associated user.");

        storedToken.RevokedAt = _dateTime.UtcNow;
        _refreshTokenRepository.Update(storedToken);

        var user = await _userRepository.GetByIdAsync(storedToken.UserId.Value, cancellationToken);
        if (user is null || user.IsDeleted || !user.IsActive)
            throw new UnauthorizedAccessException("User account is unavailable.");

        var (token, expiresAt) = _jwtService.GenerateTokenPair(
            user.Id, user.Username, user.Role?.Name ?? "", null);

        return new LoginResponseDto
        {
            AccessToken = token,
            RefreshToken = _jwtService.GenerateRefreshToken(),
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role?.Name ?? "",
        };
    }
}
