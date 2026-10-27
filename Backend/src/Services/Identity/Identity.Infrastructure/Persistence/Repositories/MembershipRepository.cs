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
}