using Domain.Entities.Common;
using Domain.Entities.Donations;
using Domain.Entities.Donors;
using Domain.Entities.Inventory;

namespace Domain.Entities.BloodUnits;

public class BloodUnit : AuditableEntity
{
    public string UnitCode { get; set; }
    public string? QrCode { get; set; }
    public Guid DonationId { get; set; }
    public Guid BloodTypeId { get; set; }
    public Guid ComponentId { get; set; }
    public Guid StatusId { get; set; }
    public Guid? StorageLocationId { get; set; }
    public int VolumeML { get; set; }
    public DateTime CollectionDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsReleased { get; set; }
    public string? Notes { get; set; }

    public Donation Donation { get; set; }
    public BloodType BloodType { get; set; }
    public BloodComponent Component { get; set; }
    public BloodUnitStatus Status { get; set; }
    public StorageLocation? StorageLocation { get; set; }
    public ICollection<BloodScreening> Screenings { get; set; } = new List<BloodScreening>();
    public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
}
