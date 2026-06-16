using Application.Common.CQRS;
using Application.DTOs.Auth;

namespace Application.Features.Auth.Commands;

public record LoginCommand(string Username, string Password) : ICommand<LoginResponseDto>;
