using Application.Common.CQRS;
using Application.DTOs.Donors;
using Application.Interfaces.Persistence;
using Domain.Entities.Donors;

namespace Application.Features.Donors.Queries;

public sealed class GetDonorByIdQueryHandler : IQueryHandler<GetDonorByIdQuery, DonorDto>
{
    private readonly IRepository<Donor> _donorRepository;

    public GetDonorByIdQueryHandler(IRepository<Donor> donorRepository)
    {
        _donorRepository = donorRepository;
    }

    public async Task<DonorDto> HandleAsync(GetDonorByIdQuery query, CancellationToken cancellationToken = default)
    {
        var donor = await _donorRepository.GetByIdAsync(query.Id, cancellationToken);
        if (donor is null || donor.IsDeleted)
            throw new KeyNotFoundException($"Donor with Id {query.Id} not found.");

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
            BloodTypeCode = donor.BloodType?.Code,
            GenderId = donor.GenderId,
            GenderName = donor.Gender?.Name,
            IsEligible = donor.IsEligible,
            LastDonationDate = donor.LastDonationDate,
        };
    }
}
