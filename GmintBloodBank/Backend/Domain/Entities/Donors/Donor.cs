using Domain.Entities.Common;
using Domain.Entities.Tenancy;
using Domain.Entities.Donations;

namespace Domain.Entities.Donors;

public class Donor : AuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string DonorCode { get; set; }
    public Guid BloodTypeId { get; set; }
    public Guid GenderId { get; set; }
    public Guid? AddressId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Identification { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsEligible { get; set; } = true;
    public DateTime? LastDonationDate { get; set; }

    public BloodType BloodType { get; set; }
    public Gender Gender { get; set; }
    public Address? Address { get; set; }
    public ICollection<DonorMedicalCondition> MedicalConditions { get; set; } = new List<DonorMedicalCondition>();
    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    public ICollection<DonationAppointment> Appointments { get; set; } = new List<DonationAppointment>();
    public ICollection<DonationEvaluation> Evaluations { get; set; } = new List<DonationEvaluation>();
}
