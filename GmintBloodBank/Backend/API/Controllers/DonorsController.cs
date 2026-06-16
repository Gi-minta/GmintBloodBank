using Application.Common.CQRS;
using Application.DTOs.Donors;
using Application.Features.Donors.Commands;
using Application.Features.Donors.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DonorsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public DonorsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DonorDto>>> GetDonors([FromQuery] GetDonorsQuery query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DonorDto>> GetDonorById(Guid id)
    {
        var result = await _queryDispatcher.QueryAsync(new GetDonorByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DonorDto>> CreateDonor([FromBody] CreateDonorCommand command)
    {
        var result = await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetDonorById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DonorDto>> UpdateDonor(Guid id, [FromBody] UpdateDonorCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }
}
