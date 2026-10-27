namespace Identity.Domain.Entities;

public sealed class Invitation : Entity
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromHours(24);

    public Guid MembershipId { get; private set; }
    public string Email { get; private set; } = null!;
    public string TokenHash { get; private set; } = null!;
    public InvitationStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    private Invitation() { }

    public static Invitation Create(Guid membershipId, string email, string tokenHash, Guid createdByUserId, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException(DomainErrors.Invitation.InvalidEmail);

        return new Invitation
        {
            MembershipId = membershipId,
            Email = email.Trim().ToLowerInvariant(),
            TokenHash = tokenHash,
            Status = InvitationStatus.Pending,
            CreatedByUserId = createdByUserId,
            CreatedAt = createdAt,
            ExpiresAt = createdAt + Lifetime
        };
    }

    public bool IsExpired(DateTime now) => now >= ExpiresAt;

    public void Accept(DateTime acceptedAt)
    {
        if (Status != InvitationStatus.Pending)
            throw new DomainException(DomainErrors.Invitation.NotPending);

        if (IsExpired(acceptedAt))
            throw new DomainException(DomainErrors.Invitation.Expired);

        Status = InvitationStatus.Accepted;
        AcceptedAt = acceptedAt;
    }

    public void Cancel(DateTime cancelledAt)
    {
        if (Status != InvitationStatus.Pending)
            return;

        Status = InvitationStatus.Cancelled;
        CancelledAt = cancelledAt;
    }
}