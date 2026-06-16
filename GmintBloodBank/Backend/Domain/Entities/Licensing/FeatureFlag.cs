using Domain.Entities.Common;

namespace Domain.Entities.Licensing;

public class FeatureFlag : AuditableEntity
{
    public string Key { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public string Scope { get; set; } = "GLOBAL";

    public ICollection<TenantFeature> TenantFeatures { get; set; } = new List<TenantFeature>();
}
