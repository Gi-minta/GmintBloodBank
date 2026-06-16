using Application.Common.CQRS;
using Application.DTOs.Donations;

namespace Application.Features.Donations.Commands;

public record CreateDonationAppointmentCommand(
    Guid DonorId,
    Guid BloodBankId,
    DateTime AppointmentDate,
    string? Notes
) : ICommand<DonationAppointmentDto>;
