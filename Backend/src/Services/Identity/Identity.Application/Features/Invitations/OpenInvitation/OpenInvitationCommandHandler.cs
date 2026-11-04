
namespace Identity.Application.Features.Invitations.OpenInvitation;

public sealed class OpenInvitationCommandHandler(
    IInvitationRepository invitations,
    IUserRepository users,
    InvitationAcceptor acceptor,
    SessionIssuer sessionIssuer,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<OpenInvitationCommand, AuthTokens>
{
    public async Task<AuthTokens> Handle(OpenInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await invitations.GetByTokenHashAsync(tokenService.HashOpaqueToken(command.Token), cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        if (await users.GetByEmailAsync(invitation.Email, cancellationToken) is not null)
            throw new ConflictException(ApplicationErrors.Invitations.LoginRequired);

        var now = timeProvider.GetUtcNow().UtcDateTime;

        var user = User.CreateInvited(invitation.Email, now);
        users.Add(user);

        await acceptor.AcceptAsync(invitation, user, now, cancellationToken);
        var tokens = await sessionIssuer.IssueAsync(user, invitation.MembershipId, now, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return tokens;
    }
}