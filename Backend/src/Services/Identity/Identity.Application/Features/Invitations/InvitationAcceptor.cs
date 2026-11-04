namespace Identity.Application.Features.Invitations;

public sealed class InvitationAcceptor(
    IMembershipRepository memberships,
    IUserRepository users,
    IAuditEntryRepository auditEntries)
{
    public async Task AcceptAsync(Invitation invitation, User user, DateTime now, CancellationToken cancellationToken)
    {
        invitation.Accept(now);

        var membership = await memberships.GetByIdAsync(invitation.MembershipId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        var details = $"{membership.Role}: {user.Email}";
        if (membership.UserId is { } previousUserId && previousUserId != user.Id)
        {
            var previous = await users.GetByIdAsync(previousUserId, cancellationToken);
            details = $"{membership.Role}: {previous?.Email} → {user.Email}";
        }

        membership.AssignTo(user.Id);

        auditEntries.Add(AuditEntry.Create(membership.TenantId, AuditAction.InvitationAccepted,
            user.Id, user.Email, membership.Id, details, now));
    }
}