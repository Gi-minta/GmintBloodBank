using Application.Common.CQRS;
using Application.DTOs.BloodUnits;
using Application.DTOs.Inventory;
using Application.Features.BloodUnits.Commands;
using Application.Features.BloodUnits.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/blood-units")]
[Authorize]
public class BloodUnitsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public BloodUnitsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<InventorySummaryDto>>> GetAvailableInventory()
    {
        var result = await _queryDispatcher.QueryAsync(new GetAvailableInventoryQuery());
        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<BloodUnitDto>> GetByCode(string code)
    {
        var result = await _queryDispatcher.QueryAsync(new GetBloodUnitByCodeQuery(code));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BloodUnitDto>> Register([FromBody] RegisterBloodUnitCommand command)
    {
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/screening")]
    public async Task<ActionResult<BloodScreeningDto>> RegisterScreening(Guid id, [FromBody] RegisterScreeningCommand command)
    {
        if (id != command.BloodUnitId) return BadRequest("ID mismatch");
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/release")]
    public async Task<ActionResult<bool>> Release(Guid id)
    {
        var result = await _commandDispatcher.SendAsync(new ReleaseBloodUnitCommand(id));
        return Ok(result);
    }

    [HttpPost("{id:guid}/discard")]
    public async Task<ActionResult<bool>> Discard(Guid id, [FromBody] DiscardBloodUnitCommand command)
    {
        if (id != command.BloodUnitId) return BadRequest("ID mismatch");
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }
}
