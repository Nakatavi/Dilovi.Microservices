namespace Identity.Application.Abstractions;

public interface IMembershipRepository
{
    void Add(Membership membership);
    Task<List<AvailableMembership>> GetAvailableAsync(Guid userId, CancellationToken cancellationToken);
    Task<Membership?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid tenantId, MembershipRole role, CancellationToken cancellationToken);
}

public sealed record AvailableMembership(Guid MembershipId, Guid TenantId, string TenantName, MembershipRole Role);