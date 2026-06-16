using Application.Common.CQRS;
using Application.DTOs.Reports;

namespace Application.Features.Reports.Queries;

public record GetBloodStockSummaryQuery : IQuery<IReadOnlyList<BloodStockSummaryDto>>;
