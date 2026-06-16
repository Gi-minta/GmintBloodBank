namespace Application.DTOs.Donations;

public class DonationEvaluationDto
{
    public Guid Id { get; set; }
    public Guid DonorId { get; set; }
    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }
    public decimal? Temperature { get; set; }
    public string? BloodPressure { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Hemoglobin { get; set; }
    public decimal? WeightKg { get; set; }
    public DateTime EvaluationDate { get; set; }
}
