using Domain.Entities.Common;

namespace Domain.Entities.Donors;

public class Gender : AuditableEntity
{
    public string Name { get; set; }

    public ICollection<Donor> Donors { get; set; } = new List<Donor>();
}
