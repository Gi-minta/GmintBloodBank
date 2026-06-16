using Domain.Entities.Common;

namespace Domain.Entities.Inventory;

public class InventoryMovementType : AuditableEntity
{
    public string Code { get; set; }
    public string Description { get; set; }
    public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
}
