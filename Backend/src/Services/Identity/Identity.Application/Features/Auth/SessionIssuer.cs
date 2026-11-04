namespace Identity.Application.Features.Auth;

public sealed class SessionIssuer(
    IMembershipRepository memberships,
    IRefreshTokenRepository refreshTokens,
    ITokenService tokenService)
{
    public async Task<AuthTokens> IssueAsync(
        User user,
        Guid? preferredMembershipId,
        DateTime now,
        CancellationToken cancellationToken,
        RefreshToken? replacing = null)
    {
        var available = user.IsSuperAdmin
            ? []
            : await memberships.GetAvailableAsync(user.Id, cancellationToken);

        AvailableMembership? current = null;
        if (!user.IsSuperAdmin)
        {
            current = available.FirstOrDefault(m => m.MembershipId == preferredMembershipId) ?? available.FirstOrDefault();
        }            

        var generated = tokenService.GenerateOpaqueToken();
        var refreshToken = RefreshToken.Create(user.Id, current?.MembershipId, generated.TokenHash, now);
        refreshTokens.Add(refreshToken);
        replacing?.Revoke(now, refreshToken.Id);

        var accessToken = tokenService.CreateAccessToken(user, current);

        return new AuthTokens(accessToken.Token, accessToken.ExpiresAt,
            generated.Token, refreshToken.ExpiresAt, current?.MembershipId, available);
    }
}