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

    public Task<List<PositionSummary>> GetPositionsAsync(Guid tenantId, DateTime now, CancellationToken cancellationToken)
    => (from m in context.Memberships
        where m.TenantId == tenantId
        join u in context.Users on m.UserId equals u.Id into holders
        from holder in holders.DefaultIfEmpty()
        let pending = context.Invitations
            .Where(i => i.MembershipId == m.Id && i.Status == InvitationStatus.Pending && i.ExpiresAt > now)
            .OrderByDescending(i => i.CreatedAt)
            .FirstOrDefault()
        orderby m.CreatedAt
        select new PositionSummary(
            m.Id,
            m.Role,
            m.Status,
            holder != null ? holder.Email : null,
            pending != null ? pending.Email : null,
            pending != null ? pending.ExpiresAt : null))
       .ToListAsync(cancellationToken);
}
