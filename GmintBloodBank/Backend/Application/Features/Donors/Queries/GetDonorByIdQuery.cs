using Application.Common.CQRS;
using Application.DTOs.Donors;

namespace Application.Features.Donors.Queries;

public record GetDonorByIdQuery(Guid Id) : IQuery<DonorDto>;
