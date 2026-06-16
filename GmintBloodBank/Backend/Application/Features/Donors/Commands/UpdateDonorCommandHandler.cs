using Application.Common.CQRS;
using Application.DTOs.Donors;
using Application.Interfaces.Persistence;
using Domain.Entities.Donors;

namespace Application.Features.Donors.Commands;

public sealed class UpdateDonorCommandHandler : ICommandHandler<UpdateDonorCommand, DonorDto>
{
    private readonly IRepository<Donor> _donorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDonorCommandHandler(IRepository<Donor> donorRepository, IUnitOfWork unitOfWork)
    {
        _donorRepository = donorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DonorDto> HandleAsync(UpdateDonorCommand command, CancellationToken cancellationToken = default)
    {
        var donor = await _donorRepository.GetByIdAsync(command.Id, cancellationToken);
        if (donor is null)
            throw new KeyNotFoundException($"Donor with Id {command.Id} not found.");

        donor.FirstName = command.FirstName;
        donor.LastName = command.LastName;
        donor.Identification = command.Identification;
        donor.DateOfBirth = command.DateOfBirth;
        donor.PhoneNumber = command.PhoneNumber;
        donor.Email = command.Email;
        donor.BloodTypeId = command.BloodTypeId;
        donor.GenderId = command.GenderId;
        donor.IsEligible = command.IsEligible;

        _donorRepository.Update(donor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DonorDto
        {
            Id = donor.Id,
            DonorCode = donor.DonorCode,
            FirstName = donor.FirstName,
            LastName = donor.LastName,
            Identification = donor.Identification,
            DateOfBirth = donor.DateOfBirth,
            PhoneNumber = donor.PhoneNumber,
            Email = donor.Email,
            BloodTypeId = donor.BloodTypeId,
            GenderId = donor.GenderId,
            IsEligible = donor.IsEligible,
        };
    }
}
