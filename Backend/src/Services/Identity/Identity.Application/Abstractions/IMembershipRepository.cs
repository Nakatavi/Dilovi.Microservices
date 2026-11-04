namespace Identity.Application.Abstractions;

public interface IMembershipRepository
{
    void Add(Membership membership);
    Task<List<AvailableMembership>> GetAvailableAsync(Guid userId, CancellationToken cancellationToken);
    Task<Membership?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid tenantId, MembershipRole role, CancellationToken cancellationToken);
    Task<List<PositionSummary>> GetPositionsAsync(Guid tenantId, DateTime now, CancellationToken cancellationToken);
}

public sealed record AvailableMembership(Guid MembershipId, Guid TenantId, string TenantName, MembershipRole Role);
public sealed record PositionSummary(
    Guid MembershipId,
    MembershipRole Role,
    MembershipStatus Status,
    string? HolderEmail,
    string? PendingInvitationEmail,
    DateTime? PendingInvitationExpiresAt);