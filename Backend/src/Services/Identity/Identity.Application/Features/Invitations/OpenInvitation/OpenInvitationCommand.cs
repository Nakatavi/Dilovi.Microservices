namespace Identity.Application.Features.Invitations.OpenInvitation;

public sealed record OpenInvitationCommand(string Token) : IRequest<AuthTokens>;