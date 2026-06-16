using Domain.Entities.Common;
using Domain.Entities.BloodUnits;

namespace Domain.Entities.Donors;

public class BloodType : AuditableEntity
{
    public string Code { get; set; }
    public string Description { get; set; }

    public ICollection<Donor> Donors { get; set; } = new List<Donor>();
    public ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
}
