namespace Application.DTOs.Inventory;

public class InventorySummaryDto
{
    public string BloodType { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public int UnitsAvailable { get; set; }
    public int TotalVolumeML { get; set; }
}
