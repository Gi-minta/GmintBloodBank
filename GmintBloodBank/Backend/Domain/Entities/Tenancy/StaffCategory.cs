using Domain.Entities.Common;

namespace Domain.Entities.Tenancy;

public class StaffCategory : AuditableEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
}
