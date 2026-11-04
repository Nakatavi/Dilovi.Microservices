
namespace Identity.Application.Features.Invitations.DeclineInvitationByToken;

public sealed class DeclineInvitationByTokenCommandHandler(
    IInvitationRepository invitations,
    IMembershipRepository memberships,
    IAuditEntryRepository auditEntries,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<DeclineInvitationByTokenCommand>
{
    public async Task Handle(DeclineInvitationByTokenCommand command, CancellationToken cancellationToken)
    {
        var invitation = await invitations.GetByTokenHashAsync(tokenService.HashOpaqueToken(command.Token), cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        var membership = await memberships.GetByIdAsync(invitation.MembershipId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        invitation.Decline(now);

        auditEntries.Add(AuditEntry.Create(membership.TenantId, AuditAction.InvitationDeclined,
            null, invitation.Email, membership.Id, $"{membership.Role}: {invitation.Email}", now));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}