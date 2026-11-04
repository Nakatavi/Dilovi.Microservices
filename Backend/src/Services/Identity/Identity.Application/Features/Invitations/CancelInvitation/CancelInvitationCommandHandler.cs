namespace Identity.Application.Features.Invitations.CancelInvitation;

public sealed class CancelInvitationCommandHandler(
    IInvitationRepository invitations,
    IMembershipRepository memberships,
    IAuditEntryRepository auditEntries,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CancelInvitationCommand>
{
    public async Task Handle(CancelInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await invitations.GetByIdAsync(command.InvitationId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        var position = await memberships.GetByIdAsync(invitation.MembershipId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Positions.NotFound);

        if (!Permissions.CanInvite(currentUser, position))
            throw new ForbiddenException(ApplicationErrors.Positions.Forbidden);

        if (invitation.Status != InvitationStatus.Pending)
            return;

        var now = timeProvider.GetUtcNow().UtcDateTime;
        invitation.Cancel(now);

        auditEntries.Add(AuditEntry.Create(position.TenantId, AuditAction.InvitationCancelled,
            currentUser.UserId, currentUser.Email, position.Id, $"{position.Role}: {invitation.Email}", now));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}