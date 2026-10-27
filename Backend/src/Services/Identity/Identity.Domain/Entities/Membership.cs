namespace Identity.Domain.Entities;

public sealed class Membership : Entity
{
    public Guid TenantId { get; private set; }
    public MembershipRole Role { get; private set; }
    public Guid? UserId { get; private set; }
    public MembershipStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    private Membership() { }

    public static Membership Create(Guid tenantId, MembershipRole role, DateTime createdAt)
        => new()
        {
            TenantId = tenantId,
            Role = role,
            Status = MembershipStatus.Vacant,
            CreatedAt = createdAt
        };

    public bool IsHeldBy(Guid userId) => Status == MembershipStatus.Active && UserId == userId;

    public void AssignTo(Guid userId)
    {
        UserId = userId;
        Status = MembershipStatus.Active;
        DeactivatedAt = null;
    }

    public void Deactivate(DateTime deactivatedAt)
    {
        if (Status == MembershipStatus.Deactivated)
            return;

        Status = MembershipStatus.Deactivated;
        DeactivatedAt = deactivatedAt;
    }
}