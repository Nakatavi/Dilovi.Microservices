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
        Error noMembershipsError,
        CancellationToken cancellationToken,
        RefreshToken? replacing = null)
    {
        var available = user.IsSuperAdmin
            ? []
            : await memberships.GetAvailableAsync(user.Id, cancellationToken);

        AvailableMembership? current = null;
        if (!user.IsSuperAdmin)
        {
            if (available.Count == 0)
                throw new UnauthorizedException(noMembershipsError);

            current = available.FirstOrDefault(m => m.MembershipId == preferredMembershipId) ?? available[0];
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