using Application.Common.CQRS;
using Application.DTOs.Inventory;
using Application.DTOs.Reports;
using Application.Features.Inventory.Commands;
using Application.Features.Inventory.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public InventoryController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InventorySummaryDto>>> GetInventory()
    {
        var result = await _queryDispatcher.QueryAsync(new GetInventoryQuery());
        return Ok(result);
    }

    [HttpGet("expiring")]
    public async Task<ActionResult<IReadOnlyList<ExpirationReportDto>>> GetExpiringUnits([FromQuery] GetExpiringUnitsQuery query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }

    [HttpPost("movements")]
    public async Task<ActionResult<InventoryMovementDto>> RegisterMovement([FromBody] RegisterMovementCommand command)
    {
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }
}
