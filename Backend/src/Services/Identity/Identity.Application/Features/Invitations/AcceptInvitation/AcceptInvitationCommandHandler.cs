namespace Identity.Application.Features.Invitations.AcceptInvitation;

public sealed class AcceptInvitationCommandHandler(
    IInvitationRepository invitations,
    IUserRepository users,
    InvitationAcceptor acceptor,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<AcceptInvitationCommand>
{
    public async Task Handle(AcceptInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await invitations.GetByIdAsync(command.InvitationId, cancellationToken);
        if (invitation is null || invitation.Email != currentUser.Email)
            throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        var user = await users.GetByIdAsync(currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        await acceptor.AcceptAsync(invitation, user, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}