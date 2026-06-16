namespace Application.DTOs.Reports;

public class ExpirationReportDto
{
    public Guid BloodUnitId { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public int DaysUntilExpiration { get; set; }
}
