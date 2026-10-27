namespace Identity.Application.Features.Auth.Logout;

public sealed class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokens,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return;

        var hash = tokenService.HashOpaqueToken(command.RefreshToken);
        var stored = await refreshTokens.GetByHashAsync(hash, cancellationToken);

        if (stored is null)
            return;

        stored.Revoke(timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}