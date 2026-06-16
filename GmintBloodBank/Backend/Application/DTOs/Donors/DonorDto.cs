using Domain.Entities.Donors;

namespace Application.DTOs.Donors;

public class DonorDto
{
    public Guid Id { get; set; }
    public string DonorCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public Guid BloodTypeId { get; set; }
    public string? BloodTypeCode { get; set; }
    public Guid GenderId { get; set; }
    public string? GenderName { get; set; }
    public bool IsEligible { get; set; }
    public DateTime? LastDonationDate { get; set; }
}
