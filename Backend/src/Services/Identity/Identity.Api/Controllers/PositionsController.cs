using Identity.Api.Contracts.Positions;
using Identity.Application.Features.Positions.CreatePosition;
using Identity.Application.Features.Positions.DeactivatePosition;
using Identity.Application.Features.Positions.ReinvitePosition;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/positions")]
public sealed class PositionsController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Chairman")]
    [HttpPost]
    public async Task<ActionResult<CreatePositionResponse>> Create(CreatePositionRequest request, CancellationToken cancellationToken)
    {
        var membershipId = await mediator.Send(new CreatePositionCommand(request.Role, request.Email), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new CreatePositionResponse(membershipId));
    }

    [Authorize(Roles = "SuperAdmin,Chairman")]
    [HttpPost("{id:guid}/invitations")]
    public async Task<IActionResult> Reinvite(Guid id, ReinvitePositionRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new ReinvitePositionCommand(id, request.Email), cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "SuperAdmin,Chairman")]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeactivatePositionCommand(id), cancellationToken);
        return NoContent();
    }
}