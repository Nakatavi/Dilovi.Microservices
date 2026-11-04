namespace Identity.Application.Features.Invitations.DeclineInvitation;

public sealed record DeclineInvitationCommand(Guid InvitationId) : IRequest;