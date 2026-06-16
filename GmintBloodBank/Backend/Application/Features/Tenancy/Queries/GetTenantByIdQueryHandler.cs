using Application.Common.CQRS;
using Application.DTOs.Tenancy;
using Application.Interfaces.Persistence;
using Domain.Entities.Tenancy;

namespace Application.Features.Tenancy.Queries;

public sealed class GetTenantByIdQueryHandler : IQueryHandler<GetTenantByIdQuery, TenantDto>
{
    private readonly IRepository<Tenant> _tenantRepository;

    public GetTenantByIdQueryHandler(IRepository<Tenant> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantDto> HandleAsync(GetTenantByIdQuery query, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(query.Id, cancellationToken);
        if (tenant is null || tenant.IsDeleted)
            throw new KeyNotFoundException($"Tenant with Id {query.Id} not found.");

        return new TenantDto
        {
            Id = tenant.Id,
            Code = tenant.Code,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
        };
    }
}
