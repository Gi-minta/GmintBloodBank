using Domain.Entities.BloodUnits;
using Domain.Entities.Common;
using Domain.Entities.Donations;
using Domain.Entities.Inventory;
using Domain.Entities.Security;

namespace Domain.Entities.Tenancy;

public class Staff : AuditableEntity
{
    public Guid BloodBankId { get; set; }
    public Guid? UserId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? AddressId { get; set; }
    public string EmployeeCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Identification { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? HireDate { get; set; }
    public bool IsActive { get; set; } = true;

    public BloodBank BloodBank { get; set; }
    public User? User { get; set; }
    public StaffCategory Category { get; set; }
    public Address? Address { get; set; }
    public ICollection<DonationEvaluation> Evaluations { get; set; } = new List<DonationEvaluation>();
    public ICollection<Donation> PerformedDonations { get; set; } = new List<Donation>();
    public ICollection<BloodScreening> Screenings { get; set; } = new List<BloodScreening>();
    public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
}
