using Domain.Entities.BloodUnits;
using Domain.Entities.Common;
using Domain.Entities.Tenancy;

namespace Domain.Entities.Inventory;

public class InventoryMovement : AuditableEntity
{
    public Guid BloodUnitId { get; set; }
    public Guid MovementTypeId { get; set; }
    public Guid PerformedById { get; set; }
    public Guid? FromLocationId { get; set; }
    public Guid? ToLocationId { get; set; }
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public BloodUnit BloodUnit { get; set; }
    public InventoryMovementType MovementType { get; set; }
    public Staff PerformedBy { get; set; }
    public StorageLocation? FromLocation { get; set; }
    public StorageLocation? ToLocation { get; set; }
}
