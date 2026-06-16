using Application.Common.CQRS;
using Application.DTOs.Auth;

namespace Application.Features.Auth.Queries;

public record GetCurrentUserQuery : IQuery<UserDto>;
