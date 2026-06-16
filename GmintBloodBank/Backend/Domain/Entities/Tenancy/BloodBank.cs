using Domain.Entities.Common;
using Domain.Entities.Donations;

namespace Domain.Entities.Tenancy;

public class BloodBank : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public Guid AddressId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public Tenant Tenant { get; set; }
    public Address Address { get; set; }
    public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
    public ICollection<DonationAppointment> Appointments { get; set; } = new List<DonationAppointment>();
    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
}
