using Application.Common.CQRS;
using Application.DTOs.Tenancy;
using Application.Interfaces.Persistence;
using Domain.Entities.Tenancy;

namespace Application.Features.Tenancy.Commands;

public sealed class CreateTenantCommandHandler : ICommandHandler<CreateTenantCommand, TenantDto>
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantCommandHandler(IRepository<Tenant> tenantRepository, IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantDto> HandleAsync(CreateTenantCommand command, CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
            Code = command.Code,
            Name = command.Name,
            IsActive = true,
        };

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TenantDto
        {
            Id = tenant.Id,
            Code = tenant.Code,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
        };
    }
}
