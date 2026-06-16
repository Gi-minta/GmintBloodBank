using Application.Common.CQRS;
using Application.DTOs.Donors;
using Application.Interfaces.Persistence;
using Domain.Entities.Donors;

namespace Application.Features.Donors.Commands;

public sealed class CreateDonorCommandHandler : ICommandHandler<CreateDonorCommand, DonorDto>
{
    private readonly IRepository<Donor> _donorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDonorCommandHandler(IRepository<Donor> donorRepository, IUnitOfWork unitOfWork)
    {
        _donorRepository = donorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DonorDto> HandleAsync(CreateDonorCommand command, CancellationToken cancellationToken = default)
    {
        var donor = new Donor
        {
            DonorCode = command.DonorCode,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Identification = command.Identification,
            DateOfBirth = command.DateOfBirth,
            PhoneNumber = command.PhoneNumber,
            Email = command.Email,
            BloodTypeId = command.BloodTypeId,
            GenderId = command.GenderId,
            IsEligible = true,
        };

        await _donorRepository.AddAsync(donor, cancellationToken);
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
