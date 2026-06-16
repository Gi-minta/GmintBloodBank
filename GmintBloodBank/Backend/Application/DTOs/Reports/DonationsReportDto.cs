namespace Application.DTOs.Reports;

public class DonationsReportDto
{
    public string Period { get; set; } = string.Empty;
    public int TotalDonations { get; set; }
    public int CompletedDonations { get; set; }
    public int RejectedDonations { get; set; }
}
