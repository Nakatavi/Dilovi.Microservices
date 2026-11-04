namespace Identity.Application.Features.Invitations.DeclineInvitation;

public sealed class DeclineInvitationCommandHandler(
    IInvitationRepository invitations,
    IMembershipRepository memberships,
    IAuditEntryRepository auditEntries,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<DeclineInvitationCommand>
{
    public async Task Handle(DeclineInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await invitations.GetByIdAsync(command.InvitationId, cancellationToken);
        if (invitation is null || invitation.Email != currentUser.Email)
            throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        var membership = await memberships.GetByIdAsync(invitation.MembershipId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        invitation.Decline(now);

        auditEntries.Add(AuditEntry.Create(membership.TenantId, AuditAction.InvitationDeclined,
            currentUser.UserId, currentUser.Email, membership.Id, $"{membership.Role}: {currentUser.Email}", now));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}