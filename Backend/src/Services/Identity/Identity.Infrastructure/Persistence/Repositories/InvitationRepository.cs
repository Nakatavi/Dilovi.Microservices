namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class InvitationRepository(IdentityDbContext context) : IInvitationRepository
{
    public void Add(Invitation invitation) => context.Invitations.Add(invitation);
    public Task<Invitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    => context.Invitations.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public Task<Invitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
        => context.Invitations.FirstOrDefaultAsync(i => i.TokenHash == tokenHash, cancellationToken);

    public Task<List<PendingInvitation>> GetPendingByEmailAsync(string email, DateTime now, CancellationToken cancellationToken)
        => (from i in context.Invitations
            join m in context.Memberships on i.MembershipId equals m.Id
            join t in context.Tenants on m.TenantId equals t.Id
            where i.Email == email
                  && i.Status == InvitationStatus.Pending
                  && i.ExpiresAt > now
                  && t.Status == TenantStatus.Active
            orderby i.CreatedAt
            select new PendingInvitation(i.Id, t.Name, m.Role, i.ExpiresAt))
           .ToListAsync(cancellationToken);

    public Task<List<Invitation>> GetPendingByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken)
    => context.Invitations
        .Where(i => i.MembershipId == membershipId && i.Status == InvitationStatus.Pending)
        .ToListAsync(cancellationToken);
}