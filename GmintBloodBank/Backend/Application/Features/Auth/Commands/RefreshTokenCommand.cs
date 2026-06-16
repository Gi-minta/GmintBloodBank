using Application.Common.CQRS;
using Application.DTOs.Auth;

namespace Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : ICommand<LoginResponseDto>;
