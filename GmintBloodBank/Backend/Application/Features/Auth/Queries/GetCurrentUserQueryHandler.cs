using Application.Common.CQRS;
using Application.DTOs.Auth;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities.Security;

namespace Application.Features.Auth.Queries;

public sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserQueryHandler(
        IRepository<User> userRepository,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<UserDto> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user is null || user.IsDeleted)
            throw new KeyNotFoundException("Current user not found.");

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            Role = user.Role?.Name ?? "",
            LastLoginAt = user.LastLoginAt,
        };
    }
}
