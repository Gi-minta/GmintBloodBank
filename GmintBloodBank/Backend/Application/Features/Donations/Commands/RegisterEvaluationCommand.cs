using Application.Common.CQRS;
using Application.DTOs.Donations;

namespace Application.Features.Donations.Commands;

public record RegisterEvaluationCommand(
    Guid DonorId,
    Guid DoctorId,
    bool IsApproved,
    string? RejectionReason,
    decimal? Temperature,
    string? BloodPressure,
    int? HeartRate,
    decimal? Hemoglobin,
    decimal? WeightKg,
    string? Notes
) : ICommand<DonationEvaluationDto>;
