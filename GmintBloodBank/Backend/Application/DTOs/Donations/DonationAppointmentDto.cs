namespace Application.DTOs.Donations;

public class DonationAppointmentDto
{
    public Guid Id { get; set; }
    public Guid DonorId { get; set; }
    public string? DonorName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
