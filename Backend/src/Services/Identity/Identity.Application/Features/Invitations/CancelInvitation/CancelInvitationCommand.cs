namespace Identity.Application.Features.Invitations.CancelInvitation;

public sealed record CancelInvitationCommand(Guid InvitationId) : IRequest;