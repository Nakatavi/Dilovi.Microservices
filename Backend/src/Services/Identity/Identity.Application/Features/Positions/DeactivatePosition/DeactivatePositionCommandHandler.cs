namespace Identity.Application.Features.Positions.DeactivatePosition;

public sealed class DeactivatePositionCommandHandler(
    IMembershipRepository memberships,
    IInvitationRepository invitations,
    IUserRepository users,
    IAuditEntryRepository auditEntries,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<DeactivatePositionCommand>
{
    public async Task Handle(DeactivatePositionCommand command, CancellationToken cancellationToken)
    {
        var position = await memberships.GetByIdAsync(command.MembershipId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Positions.NotFound);

        if (!Permissions.CanDeactivate(currentUser, position))
            throw new ForbiddenException(ApplicationErrors.Positions.Forbidden);

        if (position.Status == MembershipStatus.Deactivated)
            return;

        var now = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var pending in await invitations.GetPendingByMembershipIdAsync(position.Id, cancellationToken))
            pending.Cancel(now);

        var holder = position.UserId is { } holderId ? await users.GetByIdAsync(holderId, cancellationToken) : null;

        position.Deactivate(now);

        auditEntries.Add(AuditEntry.Create(position.TenantId, AuditAction.MembershipDeactivated,
            currentUser.UserId, currentUser.Email, position.Id, $"{position.Role}: {holder?.Email}", now));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}