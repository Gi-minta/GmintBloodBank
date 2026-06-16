using Application.Common.CQRS;
using Application.DTOs.Tenancy;
using Application.Features.Tenancy.Commands;
using Application.Features.Tenancy.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public TenantsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TenantDto>>> GetTenants()
    {
        var result = await _queryDispatcher.QueryAsync(new GetTenantsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantDto>> GetTenantById(Guid id)
    {
        var result = await _queryDispatcher.QueryAsync(new GetTenantByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TenantDto>> CreateTenant([FromBody] CreateTenantCommand command)
    {
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TenantDto>> UpdateTenant(Guid id, [FromBody] UpdateTenantCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var result = await _commandDispatcher.SendAsync(command);
        return Ok(result);
    }
}
