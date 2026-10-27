namespace Identity.Domain.Entities;

public sealed class Tenant : Entity
{
    public string Name { get; private set; } = null!;
    public TenantStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? BlockedAt { get; private set; }
    public DateTime? UnblockedAt { get; private set; }
    private Tenant() { }

    public static Tenant Create(string name, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.Tenant.NameRequired);

        return new Tenant { Name = name.Trim(), Status = TenantStatus.Active, CreatedAt = createdAt };
    }

    public void Block(DateTime blockedAt)
    {
        if (Status == TenantStatus.Blocked)
            return;
        Status = TenantStatus.Blocked;
        BlockedAt = blockedAt;
    }

    public void Unblock(DateTime unblockedAt)
    {
        if (Status == TenantStatus.Active)
            return;
        Status = TenantStatus.Active;
        UnblockedAt = unblockedAt;
    }
}
