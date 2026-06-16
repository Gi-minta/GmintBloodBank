using Domain.Entities.BloodUnits;
using Domain.Entities.Common;

namespace Domain.Entities.Inventory;

public class StorageLocation : AuditableEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
}
