namespace Identity.Application.Features.Invitations.GetMyInvitations;

public sealed class GetMyInvitationsQueryHandler(
    IInvitationRepository invitations,
    ICurrentUser currentUser,
    TimeProvider timeProvider) : IRequestHandler<GetMyInvitationsQuery, List<PendingInvitation>>
{
    public Task<List<PendingInvitation>> Handle(GetMyInvitationsQuery query, CancellationToken cancellationToken)
        => invitations.GetPendingByEmailAsync(currentUser.Email, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
}