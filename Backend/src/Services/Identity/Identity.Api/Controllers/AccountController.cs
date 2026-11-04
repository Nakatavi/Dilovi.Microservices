

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController(IMediator mediator) : ControllerBase
{
    [Authorize(Policy = AuthenticationExtensions.PasswordSetupPolicy)]
    [HttpPost("initial-password")]
    public async Task<IActionResult> SetInitialPassword(SetInitialPasswordRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new SetInitialPasswordCommand(request.Password), cancellationToken);
        return NoContent();
    }
}