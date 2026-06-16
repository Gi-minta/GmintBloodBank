using Application.Common.CQRS;
using Application.DTOs.Tenancy;

namespace Application.Features.Tenancy.Commands;

public record UpdateTenantCommand(Guid Id, string Code, string Name, bool IsActive) : ICommand<TenantDto>;
