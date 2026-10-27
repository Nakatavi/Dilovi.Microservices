namespace Identity.Application.Abstractions;

public interface IInvitationRepository
{
    void Add(Invitation invitation);
}