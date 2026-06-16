using Application.Common.CQRS;
using Application.DTOs.Licensing;
using Application.Interfaces.Persistence;
using Domain.Entities.Licensing;

namespace Application.Features.Licensing.Commands;

public sealed class AssignLicenseCommandHandler : ICommandHandler<AssignLicenseCommand, LicenseDto>
{
    private readonly IRepository<License> _licenseRepository;
    private readonly IRepository<LicensePlan> _planRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignLicenseCommandHandler(
        IRepository<License> licenseRepository,
        IRepository<LicensePlan> planRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _planRepository = planRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LicenseDto> HandleAsync(AssignLicenseCommand command, CancellationToken cancellationToken = default)
    {
        var plan = await _planRepository.GetByIdAsync(command.LicensePlanId, cancellationToken);
        if (plan is null)
            throw new KeyNotFoundException($"LicensePlan with Id {command.LicensePlanId} not found.");

        var license = new License
        {
            TenantId = command.TenantId,
            LicensePlanId = command.LicensePlanId,
            LicenseKey = command.LicenseKey,
            StartDate = command.StartDate,
            ExpirationDate = command.ExpirationDate,
            IsActive = true,
        };

        await _licenseRepository.AddAsync(license, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LicenseDto
        {
            Id = license.Id,
            LicenseKey = license.LicenseKey,
            PlanName = plan.Name,
            StartDate = license.StartDate,
            ExpirationDate = license.ExpirationDate,
            IsActive = license.IsActive,
        };
    }
}
