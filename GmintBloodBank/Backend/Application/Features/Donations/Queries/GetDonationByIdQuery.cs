using Application.Common.CQRS;
using Application.DTOs.Donations;

namespace Application.Features.Donations.Queries;

public record GetDonationByIdQuery(Guid Id) : IQuery<DonationDto>;
