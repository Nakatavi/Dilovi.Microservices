
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
        var result = await mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);

        SetRefreshTokenCookie(result);

        return Ok(new LoginResponse(result.AccessToken, result.AccessTokenExpiresAt));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        var result = await mediator.Send(new RefreshCommand(refreshToken), cancellationToken);

        SetRefreshTokenCookie(result);
        return Ok(new LoginResponse(result.AccessToken, result.AccessTokenExpiresAt));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await mediator.Send(new LogoutCommand(Request.Cookies[RefreshTokenCookie]), cancellationToken);

        Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth"
        });

        return NoContent();
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
