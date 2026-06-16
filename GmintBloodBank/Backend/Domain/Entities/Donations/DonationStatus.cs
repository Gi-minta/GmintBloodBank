using Domain.Entities.Common;

namespace Domain.Entities.Donations;

public class DonationStatus : AuditableEntity
{
    public string Code { get; set; }
    public string Description { get; set; }

    public ICollection<DonationAppointment> Appointments { get; set; } = new List<DonationAppointment>();
    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
}
