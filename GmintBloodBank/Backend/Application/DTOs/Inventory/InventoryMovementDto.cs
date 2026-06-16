namespace Application.DTOs.Inventory;

public class InventoryMovementDto
{
    public Guid Id { get; set; }
    public Guid BloodUnitId { get; set; }
    public string? UnitCode { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
    public DateTime MovementDate { get; set; }
    public string? Notes { get; set; }
}
