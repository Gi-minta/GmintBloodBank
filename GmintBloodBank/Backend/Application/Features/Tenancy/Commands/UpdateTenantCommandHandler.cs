using Application.Common.CQRS;
using Application.DTOs.Tenancy;
using Application.Interfaces.Persistence;
using Domain.Entities.Tenancy;

namespace Application.Features.Tenancy.Commands;

public sealed class UpdateTenantCommandHandler : ICommandHandler<UpdateTenantCommand, TenantDto>
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantCommandHandler(IRepository<Tenant> tenantRepository, IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantDto> HandleAsync(UpdateTenantCommand command, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(command.Id, cancellationToken);
        if (tenant is null)
            throw new KeyNotFoundException($"Tenant with Id {command.Id} not found.");

        tenant.Code = command.Code;
        tenant.Name = command.Name;
        tenant.IsActive = command.IsActive;

        _tenantRepository.Update(tenant);
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
