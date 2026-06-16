using Domain.Entities.Common;

namespace Domain.Entities.Licensing;

public class LicensePlan : AuditableEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public int MaxTenants { get; set; }
    public int MaxUsers { get; set; }

    public ICollection<License> Licenses { get; set; } = new List<License>();
}
