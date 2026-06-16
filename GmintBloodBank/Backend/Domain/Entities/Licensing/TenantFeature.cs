using Domain.Entities.Common;
using Domain.Entities.Tenancy;

namespace Domain.Entities.Licensing;

public class TenantFeature : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid FeatureFlagId { get; set; }
    public bool IsEnabled { get; set; }

    public Tenant Tenant { get; set; }
    public FeatureFlag FeatureFlag { get; set; }
}
