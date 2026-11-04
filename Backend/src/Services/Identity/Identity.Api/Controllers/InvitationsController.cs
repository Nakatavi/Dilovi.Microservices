

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/invitations")]
public sealed class InvitationsController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("open")]
    public async Task<ActionResult<LoginResponse>> Open(InvitationTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new OpenInvitationCommand(request.Token), cancellationToken);
        return Ok(Response.ToLoginResponse(result));
    }

    [AllowAnonymous]
    [HttpPost("decline")]
    public async Task<IActionResult> DeclineByToken(InvitationTokenRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeclineInvitationByTokenCommand(request.Token), cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<List<PendingInvitationResponse>>> Mine(CancellationToken cancellationToken)
    {
        var invitations = await mediator.Send(new GetMyInvitationsQuery(), cancellationToken);
        return Ok(invitations
            .Select(i => new PendingInvitationResponse(i.InvitationId, i.TenantName, i.Role.ToString(), i.ExpiresAt))
            .ToList());
    }

    [Authorize]
    [HttpPost("{id:guid}/accept")]
    public async Task<IActionResult> Accept(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new AcceptInvitationCommand(id), cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id:guid}/decline")]
    public async Task<IActionResult> Decline(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeclineInvitationCommand(id), cancellationToken);
        return NoContent();
    }
}