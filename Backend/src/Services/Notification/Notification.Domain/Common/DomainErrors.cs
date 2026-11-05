namespace Notification.Domain.Common;

public static class DomainErrors
{
    public static class EmailMessage
    {
        public static readonly Error InvalidRecipient =
            new("EmailMessage.InvalidRecipient", "Recipient email is invalid.");

        public static readonly Error EventIdRequired =
            new("EmailMessage.EventIdRequired", "Source event id is required.");
    }
}