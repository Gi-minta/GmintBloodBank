using Domain.Entities.Common;
using Domain.Entities.Donors;
using Domain.Entities.Tenancy;

namespace Domain.Entities.Donations;

public class DonationEvaluation : AuditableEntity
{
    public Guid DonorId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime EvaluationDate { get; set; } = DateTime.UtcNow;
    public decimal? Temperature { get; set; }
    public string? BloodPressure { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Hemoglobin { get; set; }
    public decimal? WeightKg { get; set; }
    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }

    public Donor Donor { get; set; }
    public Staff Doctor { get; set; }
}
