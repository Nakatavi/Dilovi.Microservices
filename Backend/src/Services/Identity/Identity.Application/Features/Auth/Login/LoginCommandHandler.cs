namespace Identity.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    SessionIssuer sessionIssuer) : IRequestHandler<LoginCommand, AuthTokens>
{
    public async Task<AuthTokens> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await GetValidUserAsync(command, cancellationToken);

        var now = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var activeToken in await refreshTokens.GetActiveByUserIdAsync(user.Id, now, cancellationToken))
            activeToken.Revoke(now);

        user.RotateSecurityStamp();

        var result = await sessionIssuer.IssueAsync(user, command.MembershipId, now,
            ApplicationErrors.Auth.InvalidCredentials, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
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


        return user;
    }
}
