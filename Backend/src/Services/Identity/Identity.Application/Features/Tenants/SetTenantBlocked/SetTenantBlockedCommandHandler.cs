namespace Identity.Application.Features.Tenants.SetTenantBlocked;

public sealed class SetTenantBlockedCommandHandler(
    ITenantRepository tenants,
    IAuditEntryRepository auditEntries,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<SetTenantBlockedCommand>
{
    public async Task Handle(SetTenantBlockedCommand command, CancellationToken cancellationToken)
    {
        var tenant = await tenants.GetByIdAsync(command.TenantId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Tenants.NotFound);

        var targetStatus = command.Blocked ? TenantStatus.Blocked : TenantStatus.Active;
        if (tenant.Status == targetStatus)
            return;

        var now = timeProvider.GetUtcNow().UtcDateTime;

        if (command.Blocked)
            tenant.Block(now);
        else
            tenant.Unblock(now);

        var action = command.Blocked ? AuditAction.TenantBlocked : AuditAction.TenantUnblocked;
        auditEntries.Add(AuditEntry.Create(tenant.Id, action,
            currentUser.UserId, currentUser.Email, null, $"Tenant: {tenant.Name}", now));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}