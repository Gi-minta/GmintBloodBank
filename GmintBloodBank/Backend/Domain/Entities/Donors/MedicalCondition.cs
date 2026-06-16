using Domain.Entities.Common;

namespace Domain.Entities.Donors;

public class MedicalCondition : AuditableEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public bool IsExclusionary { get; set; }

    public ICollection<DonorMedicalCondition> DonorMedicalConditions { get; set; } = new List<DonorMedicalCondition>();
}
