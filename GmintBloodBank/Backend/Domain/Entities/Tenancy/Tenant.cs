using Domain.Entities.Common;
using Domain.Entities.Licensing;

namespace Domain.Entities.Tenancy;

public class Tenant : AuditableEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? ConnectionString { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<BloodBank> BloodBanks { get; set; } = new List<BloodBank>();
    public ICollection<License> Licenses { get; set; } = new List<License>();
}
