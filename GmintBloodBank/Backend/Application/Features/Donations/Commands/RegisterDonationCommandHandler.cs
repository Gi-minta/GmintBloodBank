using Application.Common.CQRS;
using Application.DTOs.Donations;
using Application.Interfaces.Persistence;
using Domain.Entities.Donations;
using Domain.Entities.Donors;

namespace Application.Features.Donations.Commands;

public sealed class RegisterDonationCommandHandler : ICommandHandler<RegisterDonationCommand, DonationDto>
{
    private readonly IRepository<Donation> _donationRepository;
    private readonly IRepository<Donor> _donorRepository;
    private readonly IRepository<DonationStatus> _statusRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterDonationCommandHandler(
        IRepository<Donation> donationRepository,
        IRepository<Donor> donorRepository,
        IRepository<DonationStatus> statusRepository,
        IUnitOfWork unitOfWork)
    {
        _donationRepository = donationRepository;
        _donorRepository = donorRepository;
        _statusRepository = statusRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DonationDto> HandleAsync(RegisterDonationCommand command, CancellationToken cancellationToken = default)
    {
        var donor = await _donorRepository.GetByIdAsync(command.DonorId, cancellationToken);
        if (donor is null || donor.IsDeleted)
            throw new KeyNotFoundException($"Donor with Id {command.DonorId} not found.");

        var statuses = await _statusRepository.GetAllAsync(cancellationToken);
        var completedStatus = statuses.FirstOrDefault(s => s.Code == "COMPLETED");
        if (completedStatus is null)
            throw new InvalidOperationException("Completed status not configured.");

        var donation = new Donation
        {
            DonationCode = $"DON-{Guid.NewGuid():N}"[..16],
            DonorId = command.DonorId,
            BloodBankId = command.BloodBankId,
            EvaluationId = command.EvaluationId,
            StatusId = completedStatus.Id,
            PerformedByStaffId = command.PerformedByStaffId,
            VolumeML = command.VolumeML,
            CollectionBagCode = command.CollectionBagCode,
            Notes = command.Notes,
        };

        await _donationRepository.AddAsync(donation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DonationDto
        {
            Id = donation.Id,
            DonationCode = donation.DonationCode,
            DonorId = donation.DonorId,
            DonorName = $"{donor.FirstName} {donor.LastName}",
            DonationDate = donation.DonationDate,
            VolumeML = donation.VolumeML,
            CollectionBagCode = donation.CollectionBagCode,
            Status = completedStatus.Code,
            Notes = donation.Notes,
        };
    }
}
