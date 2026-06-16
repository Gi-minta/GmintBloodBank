using Application.Common.CQRS;
using Application.DTOs.Inventory;

namespace Application.Features.Inventory.Commands;

public record RegisterMovementCommand(
    Guid BloodUnitId,
    Guid MovementTypeId,
    Guid PerformedById,
    Guid? FromLocationId,
    Guid? ToLocationId,
    string? Notes
) : ICommand<InventoryMovementDto>;
