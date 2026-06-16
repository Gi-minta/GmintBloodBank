using Application.Common.CQRS;
using Application.Common.Models;
using Application.DTOs.Donations;

namespace Application.Features.Donations.Queries;

public record GetDonationsQuery(
    Guid? DonorId = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<PagedResult<DonationDto>>;
