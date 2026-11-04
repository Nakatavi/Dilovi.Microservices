namespace Identity.Application.Features.Invitations.AcceptInvitation;

public sealed record AcceptInvitationCommand(Guid InvitationId) : IRequest;