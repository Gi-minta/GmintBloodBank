using Domain.Entities.Common;

namespace Domain.Entities.BloodUnits;

public class BloodComponent : AuditableEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int ShelfLifeDays { get; set; }

    public ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
}
