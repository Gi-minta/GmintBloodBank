using Application.Common.CQRS;
using Application.DTOs.Licensing;
using Application.Interfaces.Persistence;
using Domain.Entities.Licensing;

namespace Application.Features.Licensing.Queries;

public sealed class GetLicenseStatusQueryHandler : IQueryHandler<GetLicenseStatusQuery, LicenseDto>
{
    private readonly IRepository<License> _licenseRepository;
    private readonly IRepository<LicensePlan> _planRepository;

    public GetLicenseStatusQueryHandler(
        IRepository<License> licenseRepository,
        IRepository<LicensePlan> planRepository)
    {
        _licenseRepository = licenseRepository;
        _planRepository = planRepository;
    }

    public async Task<LicenseDto> HandleAsync(GetLicenseStatusQuery query, CancellationToken cancellationToken = default)
    {
        var licenses = await _licenseRepository.GetAllAsync(cancellationToken);
        var license = licenses
            .Where(l => l.TenantId == query.TenantId && !l.IsDeleted)
            .OrderByDescending(l => l.StartDate)
            .FirstOrDefault();

        if (license is null)
            throw new KeyNotFoundException($"No license found for tenant {query.TenantId}.");

        var plan = await _planRepository.GetByIdAsync(license.LicensePlanId, cancellationToken);

        return new LicenseDto
        {
            Id = license.Id,
            LicenseKey = license.LicenseKey,
            PlanName = plan?.Name ?? "",
            StartDate = license.StartDate,
            ExpirationDate = license.ExpirationDate,
            IsActive = license.IsActive && license.ExpirationDate > DateTime.UtcNow,
        };
    }
}
