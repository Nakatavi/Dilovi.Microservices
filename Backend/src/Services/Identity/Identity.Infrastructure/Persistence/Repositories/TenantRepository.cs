namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class TenantRepository(IdentityDbContext context) : ITenantRepository
{
    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => context.Tenants.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public void Add(Tenant tenant) => context.Tenants.Add(tenant);
}