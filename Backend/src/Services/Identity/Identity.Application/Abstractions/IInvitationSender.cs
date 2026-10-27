namespace Identity.Application.Abstractions;

public interface IInvitationSender
{
    Task SendAsync(string email, string token, CancellationToken cancellationToken);
}