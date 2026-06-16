using Application.Common.CQRS;
using Application.Common.Models;
using Application.DTOs.Donations;
using Application.Interfaces.Persistence;
using Domain.Entities.Donations;

namespace Application.Features.Donations.Queries;

public sealed class GetDonationsQueryHandler : IQueryHandler<GetDonationsQuery, PagedResult<DonationDto>>
{
    private readonly IRepository<Donation> _donationRepository;

    public GetDonationsQueryHandler(IRepository<Donation> donationRepository)
    {
        _donationRepository = donationRepository;
    }

    public async Task<PagedResult<DonationDto>> HandleAsync(GetDonationsQuery query, CancellationToken cancellationToken = default)
    {
        var allDonations = await _donationRepository.GetAllAsync(cancellationToken);

        var filtered = allDonations
            .Where(d => !d.IsDeleted)
            .Where(d => query.DonorId is null || d.DonorId == query.DonorId)
            .ToList();

        var total = filtered.Count;
        var items = filtered
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new DonationDto
            {
                Id = d.Id,
                DonationCode = d.DonationCode,
                DonorId = d.DonorId,
                DonorName = $"{d.Donor?.FirstName} {d.Donor?.LastName}",
                DonationDate = d.DonationDate,
                VolumeML = d.VolumeML,
                CollectionBagCode = d.CollectionBagCode,
                Status = d.Status?.Code ?? "",
                BloodType = d.Donor?.BloodType?.Code,
                Notes = d.Notes,
            })
            .ToList();

        return new PagedResult<DonationDto>(items, total, query.Page, query.PageSize);
    }
}
