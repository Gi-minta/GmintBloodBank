namespace Application.DTOs.Licensing;

public class LicenseDto
{
    public Guid Id { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
}
