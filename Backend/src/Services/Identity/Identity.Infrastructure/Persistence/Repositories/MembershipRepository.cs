namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class MembershipRepository(IdentityDbContext context) : IMembershipRepository
{
    public void Add(Membership membership) => context.Memberships.Add(membership);

    public Task<List<AvailableMembership>> GetAvailableAsync(Guid userId, CancellationToken cancellationToken)
        => (from m in context.Memberships
            join t in context.Tenants on m.TenantId equals t.Id
            where m.UserId == userId
                  && m.Status == MembershipStatus.Active
                  && t.Status == TenantStatus.Active
            orderby t.Name
            select new AvailableMembership(m.Id, t.Id, t.Name, m.Role))
           .ToListAsync(cancellationToken);
    public Task<Membership?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    => context.Memberships.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid tenantId, MembershipRole role, CancellationToken cancellationToken)
    => context.Memberships.AnyAsync(m => m.TenantId == tenantId && m.Role == role, cancellationToken);
}
