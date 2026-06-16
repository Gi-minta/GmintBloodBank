using Domain.Entities.Common;

namespace Domain.Entities.Donors;

public class DonorMedicalCondition : AuditableEntity
{
    public Guid DonorId { get; set; }
    public Guid MedicalConditionId { get; set; }
    public DateTime? DiagnosisDate { get; set; }
    public string? Notes { get; set; }

    public Donor Donor { get; set; }
    public MedicalCondition MedicalCondition { get; set; }
}
