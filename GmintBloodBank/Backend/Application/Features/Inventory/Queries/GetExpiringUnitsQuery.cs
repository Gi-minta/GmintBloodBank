using Application.Common.CQRS;
using Application.DTOs.Reports;

namespace Application.Features.Inventory.Queries;

public record GetExpiringUnitsQuery(int DaysThreshold = 7) : IQuery<IReadOnlyList<ExpirationReportDto>>;
