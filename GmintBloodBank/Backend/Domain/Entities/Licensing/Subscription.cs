using Domain.Entities.Common;

namespace Domain.Entities.Licensing;

public class Subscription : AuditableEntity
{
    public Guid LicenseId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal AmountPaid { get; set; }
    public string? TransactionId { get; set; }

    public License License { get; set; }
}
