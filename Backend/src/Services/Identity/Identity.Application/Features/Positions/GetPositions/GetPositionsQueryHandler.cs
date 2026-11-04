namespace Identity.Application.Features.Positions.GetPositions;

public sealed class GetPositionsQueryHandler(
    IMembershipRepository memberships,
    ICurrentUser currentUser,
    TimeProvider timeProvider) : IRequestHandler<GetPositionsQuery, List<PositionSummary>>
{
    public Task<List<PositionSummary>> Handle(GetPositionsQuery query, CancellationToken cancellationToken)
    {
        if (!Permissions.CanViewTenant(currentUser, query.TenantId))
            throw new ForbiddenException(ApplicationErrors.Tenants.Forbidden);

        return memberships.GetPositionsAsync(query.TenantId, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
    }
}