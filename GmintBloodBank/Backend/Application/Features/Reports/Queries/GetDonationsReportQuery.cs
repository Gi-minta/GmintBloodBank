using Application.Common.CQRS;
using Application.DTOs.Reports;

namespace Application.Features.Reports.Queries;

public record GetDonationsReportQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IQuery<IReadOnlyList<DonationsReportDto>>;
