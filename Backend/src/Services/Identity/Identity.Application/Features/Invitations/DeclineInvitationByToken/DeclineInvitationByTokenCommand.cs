namespace Identity.Application.Features.Invitations.DeclineInvitationByToken;

public sealed record DeclineInvitationByTokenCommand(string Token) : IRequest;