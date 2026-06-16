using Application.Common.CQRS;
using Application.DTOs.Donors;
using Application.Interfaces.Persistence;
using Domain.Entities.Donors;

namespace Application.Features.Donors.Queries;

public sealed class GetDonorsQueryHandler : IQueryHandler<GetDonorsQuery, IReadOnlyList<DonorDto>>
{
    private readonly IRepository<Donor> _donorRepository;

    public GetDonorsQueryHandler(IRepository<Donor> donorRepository)
    {
        _donorRepository = donorRepository;
    }

    public async Task<IReadOnlyList<DonorDto>> HandleAsync(GetDonorsQuery query, CancellationToken cancellationToken = default)
    {
        var donors = await _donorRepository.GetAllAsync(cancellationToken);

        return donors
            .Where(d => !d.IsDeleted)
            .Select(d => new DonorDto
            {
                Id = d.Id,
                DonorCode = d.DonorCode,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Identification = d.Identification,
                DateOfBirth = d.DateOfBirth,
                PhoneNumber = d.PhoneNumber,
                Email = d.Email,
                BloodTypeId = d.BloodTypeId,
                BloodTypeCode = d.BloodType?.Code,
                GenderId = d.GenderId,
                GenderName = d.Gender?.Name,
                IsEligible = d.IsEligible,
                LastDonationDate = d.LastDonationDate,
            })
            .ToList();
    }
}
