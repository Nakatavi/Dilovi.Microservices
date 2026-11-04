
namespace Identity.Application.Features.Tenants.RegisterTenant;

public sealed class RegisterTenantCommandHandler(
    ITenantRepository tenants,
    IMembershipRepository memberships,
    IAuditEntryRepository auditEntries,
    InvitationIssuer invitationIssuer,
    IInvitationSender invitationSender,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<RegisterTenantCommand, Guid>
{
    public async Task<Guid> Handle(RegisterTenantCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var tenant = Tenant.Create(command.Name, now);
        tenants.Add(tenant);

        auditEntries.Add(AuditEntry.Create(tenant.Id, AuditAction.TenantCreated,
            currentUser.UserId, currentUser.Email, null, $"Tenant: {tenant.Name}", now));

        var chairmanPosition = Membership.Create(tenant.Id, MembershipRole.Chairman, now);
        memberships.Add(chairmanPosition);

        var token = await invitationIssuer.IssueAsync(chairmanPosition, command.ChairmanEmail, now, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await invitationSender.SendAsync(command.ChairmanEmail.Trim().ToLowerInvariant(), token, cancellationToken);

        return tenant.Id;
    }
}