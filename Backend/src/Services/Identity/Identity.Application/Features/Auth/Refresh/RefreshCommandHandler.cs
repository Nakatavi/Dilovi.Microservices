namespace Identity.Application.Features.Auth.Refresh;

public sealed class RefreshCommandHandler(
    IUserRepository users,
    ITenantRepository tenants,
    IRefreshTokenRepository refreshTokens,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<RefreshCommand, AuthTokens>
{
    public async Task<AuthTokens> Handle(RefreshCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            throw new UnauthorizedException(ApplicationErrors.Auth.InvalidRefreshToken);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var hash = tokenService.HashRefreshToken(command.RefreshToken);
        var stored = await refreshTokens.GetByHashAsync(hash, cancellationToken)
            ?? throw new UnauthorizedException(ApplicationErrors.Auth.InvalidRefreshToken);

        if (stored.RevokedAt is not null)
            await HandleRevokedTokenAsync(stored, now, cancellationToken);

        if (stored.ExpiresAt <= now)
            throw new UnauthorizedException(ApplicationErrors.Auth.InvalidRefreshToken);

        var user = await users.GetByIdAsync(stored.UserId, cancellationToken);
        if (user is null || !await CanSignInAsync(user, cancellationToken))
        {
            stored.Revoke(now);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException(ApplicationErrors.Auth.SessionEnded);
        }

        var generated = tokenService.GenerateRefreshToken();
        var newToken = RefreshToken.Create(user.Id, generated.TokenHash, now);
        refreshTokens.Add(newToken);
        stored.Revoke(now, replacedByTokenId: newToken.Id);

        var accessToken = tokenService.CreateAccessToken(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthTokens(accessToken.Token, accessToken.ExpiresAt, generated.Token, newToken.ExpiresAt);
    }

    private async Task HandleRevokedTokenAsync(RefreshToken stored, DateTime now, CancellationToken cancellationToken)
    {
        if (stored.ReplacedByTokenId is null)
            throw new UnauthorizedException(ApplicationErrors.Auth.SessionEnded);

        foreach (var token in await refreshTokens.GetActiveByUserIdAsync(stored.UserId, now, cancellationToken))
            token.Revoke(now);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        throw new UnauthorizedException(ApplicationErrors.Auth.InvalidRefreshToken);
    }

    private async Task<bool> CanSignInAsync(User user, CancellationToken cancellationToken)
    {
        if (user.Status != UserStatus.Active)
            return false;

        if (user.TenantId is null)
            return true;

        var tenant = await tenants.GetByIdAsync(user.TenantId.Value, cancellationToken);
        return tenant is not null && tenant.Status == TenantStatus.Active;
    }
}