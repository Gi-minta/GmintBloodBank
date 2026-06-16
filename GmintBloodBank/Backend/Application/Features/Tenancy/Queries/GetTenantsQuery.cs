using Application.Common.CQRS;
using Application.DTOs.Tenancy;

namespace Application.Features.Tenancy.Queries;

public record GetTenantsQuery : IQuery<IReadOnlyList<TenantDto>>;
