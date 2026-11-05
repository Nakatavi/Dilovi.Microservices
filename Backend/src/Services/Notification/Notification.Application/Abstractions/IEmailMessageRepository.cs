namespace Notification.Application.Abstractions;

public interface IEmailMessageRepository
{
    void Add(EmailMessage message);
    Task<EmailMessage?> GetAsync(Guid sourceEventId, string recipient, CancellationToken cancellationToken);
}