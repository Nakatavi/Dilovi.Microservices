namespace Identity.Domain.Entities;

public sealed class RefreshToken : Entity
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public Guid? MembershipId { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, Guid? membershipId, string tokenHash, DateTime createdAt)
    {
        return new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = createdAt,
            ExpiresAt = createdAt + Lifetime,
            MembershipId = membershipId
        };
    }

    public bool IsActive(DateTime now) => RevokedAt is null && now < ExpiresAt;

    public void Revoke(DateTime revokedAt, Guid? replacedByTokenId = null)
    {
        if (RevokedAt is not null)
            return;

        RevokedAt = revokedAt;
        ReplacedByTokenId = replacedByTokenId;
    }
}