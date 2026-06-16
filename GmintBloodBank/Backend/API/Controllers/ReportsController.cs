using Application.Common.CQRS;
using Application.DTOs.Reports;
using Application.Features.Reports.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public ReportsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("stock-summary")]
    public async Task<ActionResult<IReadOnlyList<BloodStockSummaryDto>>> GetStockSummary()
    {
        var result = await _queryDispatcher.QueryAsync(new GetBloodStockSummaryQuery());
        return Ok(result);
    }

    [HttpGet("donations")]
    public async Task<ActionResult<IReadOnlyList<DonationsReportDto>>> GetDonationsReport([FromQuery] GetDonationsReportQuery query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }
}
