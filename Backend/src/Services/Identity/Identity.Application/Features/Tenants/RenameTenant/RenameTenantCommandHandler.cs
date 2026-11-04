namespace Identity.Application.Features.Tenants.RenameTenant;

public sealed class RenameTenantCommandHandler(
    ITenantRepository tenants,
    IAuditEntryRepository auditEntries,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<RenameTenantCommand>
{
    public async Task Handle(RenameTenantCommand command, CancellationToken cancellationToken)
    {
        if (!Permissions.IsChairmanOf(currentUser, command.TenantId))
            throw new ForbiddenException(ApplicationErrors.Tenants.Forbidden);

        var tenant = await tenants.GetByIdAsync(command.TenantId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Tenants.NotFound);

        var oldName = tenant.Name;
        tenant.Rename(command.Name);

        if (tenant.Name == oldName)
            return;

        auditEntries.Add(AuditEntry.Create(tenant.Id, AuditAction.TenantRenamed,
            currentUser.UserId, currentUser.Email, null, $"{oldName} → {tenant.Name}",
            timeProvider.GetUtcNow().UtcDateTime));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}