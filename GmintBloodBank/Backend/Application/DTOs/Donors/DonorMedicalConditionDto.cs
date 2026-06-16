namespace Application.DTOs.Donors;

public class DonorMedicalConditionDto
{
    public Guid Id { get; set; }
    public Guid MedicalConditionId { get; set; }
    public string ConditionCode { get; set; } = string.Empty;
    public string ConditionName { get; set; } = string.Empty;
    public bool IsExclusionary { get; set; }
    public DateTime? DiagnosisDate { get; set; }
    public string? Notes { get; set; }
}
