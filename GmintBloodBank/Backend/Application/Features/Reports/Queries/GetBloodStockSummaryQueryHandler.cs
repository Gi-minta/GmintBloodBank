using Application.Common.CQRS;
using Application.DTOs.Reports;
using Application.Interfaces.Persistence;
using Domain.Entities.BloodUnits;

namespace Application.Features.Reports.Queries;

public sealed class GetBloodStockSummaryQueryHandler : IQueryHandler<GetBloodStockSummaryQuery, IReadOnlyList<BloodStockSummaryDto>>
{
    private readonly IRepository<BloodUnit> _bloodUnitRepository;

    public GetBloodStockSummaryQueryHandler(IRepository<BloodUnit> bloodUnitRepository)
    {
        _bloodUnitRepository = bloodUnitRepository;
    }

    public async Task<IReadOnlyList<BloodStockSummaryDto>> HandleAsync(GetBloodStockSummaryQuery query, CancellationToken cancellationToken = default)
    {
        var units = await _bloodUnitRepository.GetAllAsync(cancellationToken);

        var summary = units
            .Where(u => !u.IsDeleted && !u.IsReleased
                && u.ExpirationDate > DateTime.UtcNow)
            .GroupBy(u => new { BloodType = u.BloodType?.Code ?? "UNKNOWN", Component = u.Component?.Name ?? "UNKNOWN" })
            .Select(g => new BloodStockSummaryDto
            {
                BloodType = g.Key.BloodType,
                Component = g.Key.Component,
                UnitsAvailable = g.Count(),
                TotalVolumeML = g.Sum(u => u.VolumeML),
            })
            .OrderBy(s => s.BloodType)
            .ThenBy(s => s.Component)
            .ToList();

        return summary;
    }
}
