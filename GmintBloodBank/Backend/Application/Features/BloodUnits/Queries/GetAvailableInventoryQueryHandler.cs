using Application.Common.CQRS;
using Application.DTOs.Inventory;
using Application.Interfaces.Persistence;
using Domain.Entities.BloodUnits;

namespace Application.Features.BloodUnits.Queries;

public sealed class GetAvailableInventoryQueryHandler : IQueryHandler<GetAvailableInventoryQuery, IReadOnlyList<InventorySummaryDto>>
{
    private readonly IRepository<BloodUnit> _bloodUnitRepository;

    public GetAvailableInventoryQueryHandler(IRepository<BloodUnit> bloodUnitRepository)
    {
        _bloodUnitRepository = bloodUnitRepository;
    }

    public async Task<IReadOnlyList<InventorySummaryDto>> HandleAsync(GetAvailableInventoryQuery query, CancellationToken cancellationToken = default)
    {
        var units = await _bloodUnitRepository.GetAllAsync(cancellationToken);

        var available = units
            .Where(u => !u.IsDeleted && !u.IsReleased
                && u.Status?.Code == "AVAILABLE"
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

        return available;
    }
}
