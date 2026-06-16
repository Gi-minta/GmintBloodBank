namespace Application.DTOs.Donations;

public class DonationDto
{
    public Guid Id { get; set; }
    public string DonationCode { get; set; } = string.Empty;
    public Guid DonorId { get; set; }
    public string? DonorName { get; set; }
    public DateTime DonationDate { get; set; }
    public int VolumeML { get; set; }
    public string? CollectionBagCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BloodType { get; set; }
    public string? Notes { get; set; }
}
