using Domain.Entities.Common;
using Domain.Entities.Tenancy;

namespace Domain.Entities.BloodUnits;

public class BloodScreening : AuditableEntity
{
    public Guid BloodUnitId { get; set; }
    public Guid TechnicianId { get; set; }
    public DateTime ScreeningDate { get; set; } = DateTime.UtcNow;
    public string HivResult { get; set; } = "PENDING";
    public string HbsAgResult { get; set; } = "PENDING";
    public string HcvResult { get; set; } = "PENDING";
    public string VdrlResult { get; set; } = "PENDING";
    public string ChagasResult { get; set; } = "PENDING";
    public bool? IsApproved { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }

    public BloodUnit BloodUnit { get; set; }
    public Staff Technician { get; set; }
}
