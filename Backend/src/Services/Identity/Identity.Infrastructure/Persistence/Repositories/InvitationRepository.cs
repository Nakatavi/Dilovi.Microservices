namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class InvitationRepository(IdentityDbContext context) : IInvitationRepository
{
    public void Add(Invitation invitation) => context.Invitations.Add(invitation);
}