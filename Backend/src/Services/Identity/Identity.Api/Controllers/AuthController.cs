
using Identity.Application.Features.Auth;
using Identity.Application.Features.Auth.Logout;
using Identity.Application.Features.Auth.Refresh;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginCommand(request.Email, request.Password, request.MembershipId), cancellationToken);
        return Ok(ToResponse(result));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RefreshCommand(Request.Cookies[RefreshTokenCookie]), cancellationToken);
        return Ok(ToResponse(result));
    }

    [HttpPost("switch")]
    public async Task<ActionResult<LoginResponse>> Switch(SwitchMembershipRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RefreshCommand(Request.Cookies[RefreshTokenCookie], request.MembershipId), cancellationToken);
        return Ok(ToResponse(result));
    }

    private LoginResponse ToResponse(AuthTokens tokens)
    {
        SetRefreshTokenCookie(tokens);

        return new LoginResponse(
            tokens.AccessToken,
            tokens.AccessTokenExpiresAt,
            tokens.CurrentMembershipId,
            tokens.Memberships
                .Select(m => new MembershipResponse(m.MembershipId, m.TenantId, m.TenantName, m.Role.ToString()))
                .ToList());
    }
    private void SetRefreshTokenCookie(AuthTokens tokens)
    {
        Response.Cookies.Append(RefreshTokenCookie, tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth",
            Expires = tokens.RefreshTokenExpiresAt
        });
    }
}
