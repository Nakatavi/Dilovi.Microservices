namespace Identity.Application.Features.Auth.Refresh;

public sealed class RefreshCommandHandler(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    SessionIssuer sessionIssuer) : IRequestHandler<RefreshCommand, AuthTokens>
{
    public async Task<AuthTokens> Handle(RefreshCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            throw new UnauthorizedException(ApplicationErrors.Auth.InvalidRefreshToken);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var hash = tokenService.HashOpaqueToken(command.RefreshToken);
        var stored = await refreshTokens.GetByHashAsync(hash, cancellationToken)
            ?? throw new UnauthorizedException(ApplicationErrors.Auth.InvalidRefreshToken);

        if (stored.RevokedAt is not null)
            await HandleRevokedTokenAsync(stored, now, cancellationToken);

        if (stored.ExpiresAt <= now)
            throw new UnauthorizedException(ApplicationErrors.Auth.InvalidRefreshToken);

        var user = await users.GetByIdAsync(stored.UserId, cancellationToken);
        if (user is null || user.Status != UserStatus.Active)
        {
            stored.Revoke(now);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException(ApplicationErrors.Auth.SessionEnded);
        }

        var result = await sessionIssuer.IssueAsync(user, command.MembershipId ?? stored.MembershipId, now,
            ApplicationErrors.Auth.SessionEnded, cancellationToken, replacing: stored);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
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
}