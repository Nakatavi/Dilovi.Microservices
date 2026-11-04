namespace Identity.Application.Features.Positions.CreatePosition;

public sealed record CreatePositionCommand(MembershipRole Role, string Email) : IRequest<Guid>;