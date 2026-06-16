using Application.Common.CQRS;
using Application.DTOs.Donations;
using Application.Interfaces.Persistence;
using Domain.Entities.Donations;
using Domain.Entities.Donors;

namespace Application.Features.Donations.Commands;

public sealed class RegisterEvaluationCommandHandler : ICommandHandler<RegisterEvaluationCommand, DonationEvaluationDto>
{
    private readonly IRepository<DonationEvaluation> _evaluationRepository;
    private readonly IRepository<Donor> _donorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterEvaluationCommandHandler(
        IRepository<DonationEvaluation> evaluationRepository,
        IRepository<Donor> donorRepository,
        IUnitOfWork unitOfWork)
    {
        _evaluationRepository = evaluationRepository;
        _donorRepository = donorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DonationEvaluationDto> HandleAsync(RegisterEvaluationCommand command, CancellationToken cancellationToken = default)
    {
        var donor = await _donorRepository.GetByIdAsync(command.DonorId, cancellationToken);
        if (donor is null || donor.IsDeleted)
            throw new KeyNotFoundException($"Donor with Id {command.DonorId} not found.");

        var evaluation = new DonationEvaluation
        {
            DonorId = command.DonorId,
            DoctorId = command.DoctorId,
            IsApproved = command.IsApproved,
            RejectionReason = command.RejectionReason,
            Temperature = command.Temperature,
            BloodPressure = command.BloodPressure,
            HeartRate = command.HeartRate,
            Hemoglobin = command.Hemoglobin,
            WeightKg = command.WeightKg,
            Notes = command.Notes,
        };

        await _evaluationRepository.AddAsync(evaluation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DonationEvaluationDto
        {
            Id = evaluation.Id,
            DonorId = evaluation.DonorId,
            IsApproved = evaluation.IsApproved,
            RejectionReason = evaluation.RejectionReason,
            Temperature = evaluation.Temperature,
            BloodPressure = evaluation.BloodPressure,
            HeartRate = evaluation.HeartRate,
            Hemoglobin = evaluation.Hemoglobin,
            WeightKg = evaluation.WeightKg,
            EvaluationDate = evaluation.EvaluationDate,
        };
    }
}
