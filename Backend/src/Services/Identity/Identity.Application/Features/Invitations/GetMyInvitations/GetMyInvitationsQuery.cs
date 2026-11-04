namespace Identity.Application.Features.Invitations.GetMyInvitations;

public sealed record GetMyInvitationsQuery : IRequest<List<PendingInvitation>>;