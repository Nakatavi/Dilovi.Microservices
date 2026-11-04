
namespace Identity.Api.Infrastructure;

internal static class AuthResponses
{
    public const string RefreshTokenCookie = "refresh_token";

    public static LoginResponse ToLoginResponse(this HttpResponse response, AuthTokens tokens)
    {
        response.Cookies.Append(RefreshTokenCookie, tokens.RefreshToken, CookieOptions(tokens.RefreshTokenExpiresAt));

        return new LoginResponse(
            tokens.AccessToken,
            tokens.AccessTokenExpiresAt,
            tokens.CurrentMembershipId,
            tokens.Memberships
                .Select(m => new MembershipResponse(m.MembershipId, m.TenantId, m.TenantName, m.Role.ToString()))
                .ToList());
    }

    public static void DeleteRefreshTokenCookie(this HttpResponse response)
        => response.Cookies.Delete(RefreshTokenCookie, CookieOptions(expires: null));

    private static CookieOptions CookieOptions(DateTime? expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/api",
        Expires = expires
    };
}