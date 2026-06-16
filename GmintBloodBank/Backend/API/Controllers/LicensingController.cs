using Application.Common.CQRS;
using Application.DTOs.Licensing;
using Application.Features.Licensing.Commands;
using Application.Features.Licensing.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LicensingController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public LicensingController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("status")]
    public async Task<ActionResult<LicenseDto>> GetStatus([FromQuery] GetLicenseStatusQuery query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }

    [HttpGet("features")]
    public async Task<ActionResult<IReadOnlyList<FeatureFlagDto>>> GetFeatures([FromQuery] GetTenantFeatureFlagsQuery query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }

    [HttpPost("assign")]
    public async Task<ActionResult<LicenseDto>> AssignLicense([FromBody] AssignLicenseCommand command)
    {
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }

    [HttpPost("features/{id:guid}/toggle")]
    public async Task<ActionResult<FeatureFlagDto>> ToggleFeature(Guid id, [FromBody] ToggleFeatureFlagCommand command)
    {
        if (id != command.FeatureFlagId) return BadRequest("ID mismatch");
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }
}
