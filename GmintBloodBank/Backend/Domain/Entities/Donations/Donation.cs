using Domain.Entities.Common;
using Domain.Entities.BloodUnits;
using Domain.Entities.Donors;
using Domain.Entities.Tenancy;

namespace Domain.Entities.Donations;

public class Donation : AuditableEntity
{
    public string DonationCode { get; set; }
    public Guid DonorId { get; set; }
    public Guid BloodBankId { get; set; }
    public Guid? EvaluationId { get; set; }
    public Guid StatusId { get; set; }
    public Guid? PerformedByStaffId { get; set; }
    public DateTime DonationDate { get; set; } = DateTime.UtcNow;
    public int VolumeML { get; set; }
    public string? CollectionBagCode { get; set; }
    public string? Notes { get; set; }

    public Donor Donor { get; set; }
    public BloodBank BloodBank { get; set; }
    public DonationEvaluation? Evaluation { get; set; }
    public DonationStatus Status { get; set; }
    public Staff? PerformedByStaff { get; set; }
    public ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
}
