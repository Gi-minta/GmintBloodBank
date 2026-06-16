using Domain.Entities.Common;
using Domain.Entities.Donors;

namespace Domain.Entities.Tenancy;

public class Address : AuditableEntity
{
    public string Country { get; set; }
    public string? State { get; set; }
    public string City { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public ICollection<Donor> Donors { get; set; } = new List<Donor>();
    public ICollection<BloodBank> BloodBanks { get; set; } = new List<BloodBank>();
    public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
}
