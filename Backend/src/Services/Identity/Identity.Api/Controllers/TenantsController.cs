using Identity.Api.Contracts.Tenants;
using Identity.Application.Features.Positions.GetPositions;
using Identity.Application.Features.Tenants.RegisterTenant;
using Identity.Application.Features.Tenants.RenameTenant;
using Identity.Application.Features.Tenants.SetTenantBlocked;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/tenants")]
public sealed class TenantsController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<ActionResult<RegisterTenantResponse>> Register(RegisterTenantRequest request, CancellationToken cancellationToken)
    {
        var tenantId = await mediator.Send(new RegisterTenantCommand(request.Name, request.ChairmanEmail), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new RegisterTenantResponse(tenantId));
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("{id:guid}/block")]
    public async Task<IActionResult> Block(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new SetTenantBlockedCommand(id, Blocked: true), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("{id:guid}/unblock")]
    public async Task<IActionResult> Unblock(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new SetTenantBlockedCommand(id, Blocked: false), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Chairman")]
    [HttpPut("{id:guid}/name")]
    public async Task<IActionResult> Rename(Guid id, RenameTenantRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new RenameTenantCommand(id, request.Name), cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id:guid}/positions")]
    public async Task<ActionResult<List<PositionSummary>>> Positions(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetPositionsQuery(id), cancellationToken));
}