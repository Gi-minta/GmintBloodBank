using Application.Common.CQRS;
using Application.DTOs.Donations;

namespace Application.Features.Donations.Commands;

public record RegisterDonationCommand(
    Guid DonorId,
    Guid BloodBankId,
    Guid? EvaluationId,
    Guid? PerformedByStaffId,
    int VolumeML,
    string? CollectionBagCode,
    string? Notes
) : ICommand<DonationDto>;
