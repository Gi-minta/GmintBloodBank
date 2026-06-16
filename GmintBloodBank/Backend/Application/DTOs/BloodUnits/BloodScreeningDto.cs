namespace Application.DTOs.BloodUnits;

public class BloodScreeningDto
{
    public Guid Id { get; set; }
    public Guid BloodUnitId { get; set; }
    public string? TechnicianName { get; set; }
    public DateTime ScreeningDate { get; set; }
    public string HivResult { get; set; } = string.Empty;
    public string HbsAgResult { get; set; } = string.Empty;
    public string HcvResult { get; set; } = string.Empty;
    public string VdrlResult { get; set; } = string.Empty;
    public string ChagasResult { get; set; } = string.Empty;
    public bool? IsApproved { get; set; }
    public string? Notes { get; set; }
}
