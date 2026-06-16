using Domain.Entities.Common;
using Domain.Entities.Donors;
using Domain.Entities.Tenancy;

namespace Domain.Entities.Donations;

public class DonationAppointment : AuditableEntity
{
    public Guid DonorId { get; set; }
    public Guid BloodBankId { get; set; }
    public Guid StatusId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Notes { get; set; }

    public Donor Donor { get; set; }
    public BloodBank BloodBank { get; set; }
    public DonationStatus Status { get; set; }
}
