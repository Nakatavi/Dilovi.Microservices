
namespace Identity.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IUserRepository users,
    ITenantRepository tenants,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<LoginCommand, LoginResult>
{
    public async ValueTask<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await GetValidUserAsync(command, cancellationToken);

        var now = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var activeToken in await refreshTokens.GetActiveByUserIdAsync(user.Id, now, cancellationToken))
            activeToken.Revoke(now);

        user.RotateSecurityStamp();

        var generated = tokenService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, generated.TokenHash, now);
        refreshTokens.Add(refreshToken);

        var accessToken = tokenService.CreateAccessToken(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken.Token, accessToken.ExpiresAt, generated.Token, refreshToken.ExpiresAt);
    }

    private async Task<User> GetValidUserAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailAsync(email, cancellationToken);

        if (user is null
            || user.Status != UserStatus.Active
            || user.PasswordHash is null
            || !passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedException(ApplicationErrors.Auth.InvalidCredentials);

        if (user.TenantId is not null)
        {
            var tenant = await tenants.GetByIdAsync(user.TenantId.Value, cancellationToken);
            if (tenant is null || tenant.Status == TenantStatus.Blocked)
                throw new UnauthorizedException(ApplicationErrors.Auth.InvalidCredentials);
        }

        return user;
    }
}
