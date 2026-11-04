namespace Identity.Application.Abstractions;

public interface IInvitationRepository
{
    void Add(Invitation invitation);
    Task<Invitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Invitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<List<PendingInvitation>> GetPendingByEmailAsync(string email, DateTime now, CancellationToken cancellationToken);
    Task<List<Invitation>> GetPendingByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken);
}

public sealed record PendingInvitation(Guid InvitationId, string TenantName, MembershipRole Role, DateTime ExpiresAt);