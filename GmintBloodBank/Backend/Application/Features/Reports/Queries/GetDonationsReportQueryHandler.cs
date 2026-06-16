using Application.Common.CQRS;
using Application.DTOs.Reports;
using Application.Interfaces.Persistence;
using Domain.Entities.Donations;

namespace Application.Features.Reports.Queries;

public sealed class GetDonationsReportQueryHandler : IQueryHandler<GetDonationsReportQuery, IReadOnlyList<DonationsReportDto>>
{
    private readonly IRepository<Donation> _donationRepository;

    public GetDonationsReportQueryHandler(IRepository<Donation> donationRepository)
    {
        _donationRepository = donationRepository;
    }

    public async Task<IReadOnlyList<DonationsReportDto>> HandleAsync(GetDonationsReportQuery query, CancellationToken cancellationToken = default)
    {
        var allDonations = await _donationRepository.GetAllAsync(cancellationToken);

        var from = query.FromDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = query.ToDate ?? DateTime.UtcNow;

        var filtered = allDonations
            .Where(d => !d.IsDeleted && d.DonationDate >= from && d.DonationDate <= to)
            .GroupBy(d => d.DonationDate.ToString("yyyy-MM"))
            .Select(g => new DonationsReportDto
            {
                Period = g.Key,
                TotalDonations = g.Count(),
                CompletedDonations = g.Count(d => d.Status?.Code == "COMPLETED"),
                RejectedDonations = g.Count(d => d.Status?.Code == "REJECTED"),
            })
            .OrderBy(r => r.Period)
            .ToList();

        return filtered;
    }
}
