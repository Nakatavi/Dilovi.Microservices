using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Invitations;

internal sealed class LoggingInvitationSender(
    IOptions<InvitationOptions> options,
    ILogger<LoggingInvitationSender> logger) : IInvitationSender
{
    public Task SendAsync(string email, string token, CancellationToken cancellationToken)
    {
        var link = $"{options.Value.AcceptUrl}?token={Uri.EscapeDataString(token)}";
        logger.LogInformation("Invitation for {Email}: {Link}", email, link);
        return Task.CompletedTask;
    }
}