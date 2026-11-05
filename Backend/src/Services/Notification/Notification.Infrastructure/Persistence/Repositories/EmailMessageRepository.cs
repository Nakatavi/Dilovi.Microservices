namespace Notification.Infrastructure.Persistence.Repositories;

internal sealed class EmailMessageRepository(NotificationDbContext context) : IEmailMessageRepository
{
    public void Add(EmailMessage message) => context.EmailMessages.Add(message);

    public Task<EmailMessage?> GetAsync(Guid sourceEventId, string recipient, CancellationToken cancellationToken)
    {
        var normalized = recipient.Trim().ToLowerInvariant();
        return context.EmailMessages
            .FirstOrDefaultAsync(m => m.SourceEventId == sourceEventId && m.Recipient == normalized, cancellationToken);
    }
}