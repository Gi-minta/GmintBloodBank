using Application.Common.CQRS;
using Application.DTOs.Tenancy;
using Application.Interfaces.Persistence;
using Domain.Entities.Tenancy;

namespace Application.Features.Tenancy.Queries;

public sealed class GetTenantsQueryHandler : IQueryHandler<GetTenantsQuery, IReadOnlyList<TenantDto>>
{
    private readonly IRepository<Tenant> _tenantRepository;

    public GetTenantsQueryHandler(IRepository<Tenant> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<IReadOnlyList<TenantDto>> HandleAsync(GetTenantsQuery query, CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);

        return tenants
            .Where(t => !t.IsDeleted)
            .Select(t => new TenantDto
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name,
                IsActive = t.IsActive,
            })
            .ToList();
    }
}
