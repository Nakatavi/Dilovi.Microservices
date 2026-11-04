namespace Identity.Application.Features.Positions.DeactivatePosition;

public sealed record DeactivatePositionCommand(Guid MembershipId) : IRequest;