using Domain.Entities.Common;
using Domain.Entities.Tenancy;

namespace Domain.Entities.Licensing;

public class License : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid LicensePlanId { get; set; }
    public string LicenseKey { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Tenant Tenant { get; set; }
    public LicensePlan Plan { get; set; }
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
