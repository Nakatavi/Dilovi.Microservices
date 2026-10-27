namespace Identity.Application.Features.Tenants.RegisterTenant;

public sealed class RegisterTenantCommandHandler(
    ITenantRepository tenants,
    IMembershipRepository memberships,
    IInvitationRepository invitations,
    IAuditEntryRepository auditEntries,
    ITokenService tokenService,
    IInvitationSender invitationSender,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<RegisterTenantCommand, Guid>
{
    public async Task<Guid> Handle(RegisterTenantCommand command, CancellationToken cancellationToken)
    {
        var email = command.ChairmanEmail.Trim().ToLowerInvariant();


        var now = timeProvider.GetUtcNow().UtcDateTime;

        var tenant = Tenant.Create(command.Name, now);
        tenants.Add(tenant);

        var chairmanPosition = Membership.Create(tenant.Id, MembershipRole.Chairman, now);
        memberships.Add(chairmanPosition);

        var token = tokenService.GenerateOpaqueToken();
        invitations.Add(Invitation.Create(chairmanPosition.Id, email, token.TokenHash, currentUser.UserId, now));

        auditEntries.Add(AuditEntry.Create(tenant.Id, AuditAction.TenantCreated,
            currentUser.UserId, currentUser.Email, null, $"Tenant: {tenant.Name}", now));
        auditEntries.Add(AuditEntry.Create(tenant.Id, AuditAction.InvitationSent,
            currentUser.UserId, currentUser.Email, chairmanPosition.Id, $"Chairman: {email}", now));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await invitationSender.SendAsync(email, token.Token, cancellationToken);

        return tenant.Id;
    }
}