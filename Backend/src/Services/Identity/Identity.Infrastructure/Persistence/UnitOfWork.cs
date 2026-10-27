
namespace Identity.Infrastructure.Persistence;

internal sealed class UnitOfWork(IdentityDbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);
}