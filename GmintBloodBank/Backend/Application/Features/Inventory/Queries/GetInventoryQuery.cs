using Application.Common.CQRS;
using Application.DTOs.Inventory;

namespace Application.Features.Inventory.Queries;

public record GetInventoryQuery : IQuery<IReadOnlyList<InventorySummaryDto>>;
