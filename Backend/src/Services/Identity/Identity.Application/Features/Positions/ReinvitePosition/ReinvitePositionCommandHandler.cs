using Identity.Application.Features.Invitations;

namespace Identity.Application.Features.Positions.ReinvitePosition;

public sealed class ReinvitePositionCommandHandler(
    IMembershipRepository memberships,
    IUserRepository users,
    InvitationIssuer invitationIssuer,
    IInvitationSender invitationSender,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<ReinvitePositionCommand>
{
    public async Task Handle(ReinvitePositionCommand command, CancellationToken cancellationToken)
    {
        var position = await memberships.GetByIdAsync(command.MembershipId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Positions.NotFound);

        if (!Permissions.CanInvite(currentUser, position))
            throw new ForbiddenException(ApplicationErrors.Positions.Forbidden);

        var email = command.Email.Trim().ToLowerInvariant();
        await EnsureNotAlreadyHeldByAsync(position, email, cancellationToken);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var token = await invitationIssuer.IssueAsync(position, email, now, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await invitationSender.SendAsync(email, token, cancellationToken);
    }

    private async Task EnsureNotAlreadyHeldByAsync(Membership position, string email, CancellationToken cancellationToken)
    {
        if (position.Status != MembershipStatus.Active || position.UserId is not { } holderId)
            return;

        var holder = await users.GetByIdAsync(holderId, cancellationToken);
        if (holder?.Email == email)
            throw new ConflictException(ApplicationErrors.Positions.AlreadyHeld);
    }
}