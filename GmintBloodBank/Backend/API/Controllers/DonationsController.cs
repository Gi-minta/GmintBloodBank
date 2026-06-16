using Application.Common.CQRS;
using Application.Common.Models;
using Application.DTOs.Donations;
using Application.Features.Donations.Commands;
using Application.Features.Donations.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DonationsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public DonationsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<DonationDto>>> GetDonations([FromQuery] GetDonationsQuery query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DonationDto>> GetDonationById(Guid id)
    {
        var result = await _queryDispatcher.QueryAsync(new GetDonationByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DonationDto>> RegisterDonation([FromBody] RegisterDonationCommand command)
    {
        var result = await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetDonationById), new { id = result.Id }, result);
    }

    [HttpPost("appointments")]
    public async Task<ActionResult<DonationAppointmentDto>> CreateAppointment([FromBody] CreateDonationAppointmentCommand command)
    {
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }

    [HttpPost("evaluations")]
    public async Task<ActionResult<DonationEvaluationDto>> RegisterEvaluation([FromBody] RegisterEvaluationCommand command)
    {
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }
}
