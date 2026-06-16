namespace Application.DTOs.BloodUnits;

public class BloodUnitDto
{
    public Guid Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string? QrCode { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int VolumeML { get; set; }
    public DateTime CollectionDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsReleased { get; set; }
}
