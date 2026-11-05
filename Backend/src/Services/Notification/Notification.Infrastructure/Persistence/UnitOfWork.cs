namespace Notification.Infrastructure.Persistence;

internal sealed class UnitOfWork(NotificationDbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);
}