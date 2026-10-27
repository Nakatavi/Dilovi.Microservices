namespace Identity.Infrastructure.Invitations;

public sealed class InvitationOptions
{
    public const string SectionName = "Invitations";

    public string AcceptUrl { get; init; } = string.Empty;
}