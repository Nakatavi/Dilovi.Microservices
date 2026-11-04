

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginCommand(request.Email, request.Password, request.MembershipId), cancellationToken);
        return Ok(Response.ToLoginResponse(result));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RefreshCommand(Request.Cookies[AuthResponses.RefreshTokenCookie]), cancellationToken);
        return Ok(Response.ToLoginResponse(result));
    }

    [HttpPost("switch")]
    public async Task<ActionResult<LoginResponse>> Switch(SwitchMembershipRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RefreshCommand(Request.Cookies[AuthResponses.RefreshTokenCookie], request.MembershipId), cancellationToken);
        return Ok(Response.ToLoginResponse(result));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await mediator.Send(new LogoutCommand(Request.Cookies[AuthResponses.RefreshTokenCookie]), cancellationToken);
        Response.DeleteRefreshTokenCookie();
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        UserId = User.FindFirst("sub")?.Value,
        Email = User.FindFirst("email")?.Value,
        Role = User.FindFirst("role")?.Value,
        TenantId = User.FindFirst("tenant_id")?.Value,
        MembershipId = User.FindFirst("membership_id")?.Value
    });
}