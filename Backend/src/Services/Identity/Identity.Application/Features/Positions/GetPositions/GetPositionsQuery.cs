namespace Identity.Application.Features.Positions.GetPositions;

public sealed record GetPositionsQuery(Guid TenantId) : IRequest<List<PositionSummary>>;