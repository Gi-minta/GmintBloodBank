using Application.Common.CQRS;
using Application.DTOs.Auth;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities.Security;

namespace Application.Features.Auth.Commands;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponseDto>
{
    private readonly IRepository<User> _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IDateTimeProvider _dateTime;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IRepository<User> userRepository,
        IJwtService jwtService,
        IDateTimeProvider dateTime,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _dateTime = dateTime;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u =>
            u.Username == command.Username && !u.IsDeleted);

        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        user.LastLoginAt = _dateTime.UtcNow;
        _userRepository.Update(user);

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
