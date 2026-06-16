using Application.Common.CQRS;
using Application.DTOs.BloodUnits;

namespace Application.Features.BloodUnits.Commands;

public record RegisterBloodUnitCommand(
    Guid DonationId,
    Guid BloodTypeId,
    Guid ComponentId,
    int VolumeML,
    DateTime CollectionDate,
    DateTime ExpirationDate,
    string? Notes
) : ICommand<BloodUnitDto>;
