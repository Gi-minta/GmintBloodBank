using Application.Common.CQRS;
using Application.DTOs.Inventory;

namespace Application.Features.BloodUnits.Queries;

public record GetAvailableInventoryQuery : IQuery<IReadOnlyList<InventorySummaryDto>>;
