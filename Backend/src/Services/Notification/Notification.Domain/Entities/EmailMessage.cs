
namespace Notification.Domain.Entities;

public sealed class EmailMessage : Entity
{
    private const int MaxErrorLength = 2000;

    public Guid SourceEventId { get; private set; }
    public EmailType Type { get; private set; }
    public string Recipient { get; private set; } = null!;
    public EmailStatus Status { get; private set; }
    public int Attempts { get; private set; }
    public string? LastError { get; private set; }
    public string? ProviderMessageId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }

    private EmailMessage() { }

    public bool IsSent => Status == EmailStatus.Sent;

    public static EmailMessage Create(Guid sourceEventId, EmailType type, string recipient, DateTime createdAt)
    {
        if (sourceEventId == Guid.Empty)
            throw new DomainException(DomainErrors.EmailMessage.EventIdRequired);

        var normalized = recipient?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized) || !normalized.Contains('@'))
            throw new DomainException(DomainErrors.EmailMessage.InvalidRecipient);

        return new EmailMessage
        {
            SourceEventId = sourceEventId,
            Type = type,
            Recipient = normalized,
            Status = EmailStatus.Pending,
            CreatedAt = createdAt
        };
    }

    public void MarkSent(string? providerMessageId, DateTime sentAt)
    {
        if (IsSent) return;

        Attempts++;
        Status = EmailStatus.Sent;
        ProviderMessageId = providerMessageId;
        SentAt = sentAt;
        LastAttemptAt = sentAt;
        LastError = null;
    }

    public void MarkFailed(string error, DateTime failedAt)
    {
        if (IsSent) return;

        Attempts++;
        Status = EmailStatus.Failed;
        LastAttemptAt = failedAt;
        LastError = error.Length > MaxErrorLength ? error[..MaxErrorLength] : error;
    }
}