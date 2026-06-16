namespace Application.DTOs.Licensing;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid LicenseId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal AmountPaid { get; set; }
    public string? TransactionId { get; set; }
}
