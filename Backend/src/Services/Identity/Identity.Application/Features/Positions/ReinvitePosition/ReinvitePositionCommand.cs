namespace Identity.Application.Features.Positions.ReinvitePosition;

public sealed record ReinvitePositionCommand(Guid MembershipId, string Email) : IRequest;