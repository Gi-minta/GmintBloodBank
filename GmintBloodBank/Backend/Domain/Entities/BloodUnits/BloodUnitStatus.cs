using Domain.Entities.Common;

namespace Domain.Entities.BloodUnits;

public class BloodUnitStatus : AuditableEntity
{
    public string Code { get; set; }
    public string Description { get; set; }

    public ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
}
