using Application.Common.CQRS;
using Application.DTOs.Reports;
using Application.Interfaces.Persistence;
using Domain.Entities.BloodUnits;

namespace Application.Features.Reports.Queries;

public sealed class GetExpiringUnitsReportQueryHandler : IQueryHandler<GetExpiringUnitsReportQuery, IReadOnlyList<ExpirationReportDto>>
{
    private readonly IRepository<BloodUnit> _bloodUnitRepository;

    public GetExpiringUnitsReportQueryHandler(IRepository<BloodUnit> bloodUnitRepository)
    {
        _bloodUnitRepository = bloodUnitRepository;
    }

    public async Task<IReadOnlyList<ExpirationReportDto>> HandleAsync(GetExpiringUnitsReportQuery query, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(query.DaysThreshold);

        var units = await _bloodUnitRepository.GetAllAsync(cancellationToken);

        var expiring = units
            .Where(u => !u.IsDeleted && !u.IsReleased
                && u.ExpirationDate > now
                && u.ExpirationDate <= threshold)
            .Select(u => new ExpirationReportDto
            {
                BloodUnitId = u.Id,
                UnitCode = u.UnitCode,
                BloodType = u.BloodType?.Code ?? "",
                ExpirationDate = u.ExpirationDate,
                DaysUntilExpiration = (int)(u.ExpirationDate - now).TotalDays,
            })
            .OrderBy(e => e.ExpirationDate)
            .ToList();

        return expiring;
    }
}
