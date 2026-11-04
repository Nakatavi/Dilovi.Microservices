
namespace Identity.Api.Contracts.Positions;

public sealed record CreatePositionRequest(MembershipRole Role, string Email);