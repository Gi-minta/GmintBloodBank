using Application.Common.CQRS;
using Application.DTOs.Inventory;
using Application.Interfaces.Persistence;
using Domain.Entities.BloodUnits;

namespace Application.Features.Inventory.Queries;

public sealed class GetInventoryQueryHandler : IQueryHandler<GetInventoryQuery, IReadOnlyList<InventorySummaryDto>>
{
    private readonly IRepository<BloodUnit> _bloodUnitRepository;

    public GetInventoryQueryHandler(IRepository<BloodUnit> bloodUnitRepository)
    {
        _bloodUnitRepository = bloodUnitRepository;
    }

    public async Task<IReadOnlyList<InventorySummaryDto>> HandleAsync(GetInventoryQuery query, CancellationToken cancellationToken = default)
    {
        var units = await _bloodUnitRepository.GetAllAsync(cancellationToken);

        var summary = units
            .Where(u => !u.IsDeleted && !u.IsReleased
                && u.ExpirationDate > DateTime.UtcNow)
            .GroupBy(u => new { BloodType = u.BloodType?.Code ?? "UNKNOWN", Component = u.Component?.Name ?? "UNKNOWN" })
            .Select(g => new InventorySummaryDto
            {
                BloodType = g.Key.BloodType,
                Component = g.Key.Component,
                UnitsAvailable = g.Count(),
                TotalVolumeML = g.Sum(u => u.VolumeML),
            })
            .ToList();

        return summary;
    }
}
