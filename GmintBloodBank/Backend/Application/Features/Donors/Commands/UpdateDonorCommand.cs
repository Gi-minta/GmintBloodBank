using Application.Common.CQRS;
using Application.DTOs.Donors;

namespace Application.Features.Donors.Commands;

public record UpdateDonorCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Identification,
    DateTime DateOfBirth,
    Guid BloodTypeId,
    Guid GenderId,
    string? PhoneNumber,
    string? Email,
    bool IsEligible
) : ICommand<DonorDto>;
