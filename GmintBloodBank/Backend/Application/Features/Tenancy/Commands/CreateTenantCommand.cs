using Application.Common.CQRS;
using Application.DTOs.Tenancy;

namespace Application.Features.Tenancy.Commands;

public record CreateTenantCommand(string Code, string Name) : ICommand<TenantDto>;
