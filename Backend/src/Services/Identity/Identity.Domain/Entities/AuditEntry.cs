namespace Identity.Domain.Entities;

public sealed class AuditEntry : Entity
{
    public Guid? TenantId { get; private set; }
    public AuditAction Action { get; private set; }
    public Guid ActorUserId { get; private set; }
    public string ActorEmail { get; private set; } = null!;
    public Guid? TargetUserId { get; private set; }
    public string? Details { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private AuditEntry() { }

    public static AuditEntry Create(
        Guid? tenantId,
        AuditAction action,
        Guid actorUserId,
        string actorEmail,
        Guid? targetUserId,
        string? details,
        DateTime occurredAt)
    {
        return new AuditEntry
        {
            TenantId = tenantId,
            Action = action,
            ActorUserId = actorUserId,
            ActorEmail = actorEmail,
            TargetUserId = targetUserId,
            Details = details,
            OccurredAt = occurredAt
        };
    }
}