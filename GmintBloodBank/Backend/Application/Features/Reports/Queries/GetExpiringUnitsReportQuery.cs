using Application.Common.CQRS;
using Application.DTOs.Reports;

namespace Application.Features.Reports.Queries;

public record GetExpiringUnitsReportQuery(int DaysThreshold = 7) : IQuery<IReadOnlyList<ExpirationReportDto>>;
