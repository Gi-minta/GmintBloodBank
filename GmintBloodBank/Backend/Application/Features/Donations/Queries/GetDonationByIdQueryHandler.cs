using Application.Common.CQRS;
using Application.DTOs.Donations;
using Application.Interfaces.Persistence;
using Domain.Entities.Donations;

namespace Application.Features.Donations.Queries;

public sealed class GetDonationByIdQueryHandler : IQueryHandler<GetDonationByIdQuery, DonationDto>
{
    private readonly IRepository<Donation> _donationRepository;

    public GetDonationByIdQueryHandler(IRepository<Donation> donationRepository)
    {
        _donationRepository = donationRepository;
    }

    public async Task<DonationDto> HandleAsync(GetDonationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var donation = await _donationRepository.GetByIdAsync(query.Id, cancellationToken);
        if (donation is null || donation.IsDeleted)
            throw new KeyNotFoundException($"Donation with Id {query.Id} not found.");

        return new DonationDto
        {
            Id = donation.Id,
            DonationCode = donation.DonationCode,
            DonorId = donation.DonorId,
            DonorName = $"{donation.Donor?.FirstName} {donation.Donor?.LastName}",
            DonationDate = donation.DonationDate,
            VolumeML = donation.VolumeML,
            CollectionBagCode = donation.CollectionBagCode,
            Status = donation.Status?.Code ?? "",
            BloodType = donation.Donor?.BloodType?.Code,
            Notes = donation.Notes,
        };
    }
}
