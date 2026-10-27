namespace Identity.Api.Contracts.Auth;

public sealed record MembershipResponse(Guid MembershipId, Guid TenantId, string TenantName, string Role);