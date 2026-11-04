namespace Identity.Application.Features.Tenants.SetTenantBlocked;

public sealed record SetTenantBlockedCommand(Guid TenantId, bool Blocked) : IRequest;