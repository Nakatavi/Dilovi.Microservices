namespace Identity.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
    Guid? TenantId { get; }
    Guid? MembershipId { get; }
}