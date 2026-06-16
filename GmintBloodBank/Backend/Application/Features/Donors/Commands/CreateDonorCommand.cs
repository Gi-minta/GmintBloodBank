using Application.Common.CQRS;
using Application.DTOs.Donors;

namespace Application.Features.Donors.Commands;

public record CreateDonorCommand(
    string DonorCode,
    string FirstName,
    string LastName,
    string Identification,
    DateTime DateOfBirth,
    Guid BloodTypeId,
    Guid GenderId,
    string? PhoneNumber,
    string? Email
) : ICommand<DonorDto>;
