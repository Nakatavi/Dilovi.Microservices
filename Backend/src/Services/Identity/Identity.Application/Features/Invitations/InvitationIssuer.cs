namespace Identity.Application.Features.Invitations;

public sealed class InvitationIssuer(
    IInvitationRepository invitations,
    IAuditEntryRepository auditEntries,
    ITokenService tokenService,
    ICurrentUser currentUser)
{
    public async Task<string> IssueAsync(Membership membership, string email, DateTime now, CancellationToken cancellationToken)
    {
        foreach (var pending in await invitations.GetPendingByMembershipIdAsync(membership.Id, cancellationToken))
            pending.Cancel(now);

        var token = tokenService.GenerateOpaqueToken();
        var invitation = Invitation.Create(membership.Id, email, token.TokenHash, currentUser.UserId, now);
        invitations.Add(invitation);

        auditEntries.Add(AuditEntry.Create(membership.TenantId, AuditAction.InvitationSent,
            currentUser.UserId, currentUser.Email, membership.Id, $"{membership.Role}: {invitation.Email}", now));

        return token.Token;
    }
}