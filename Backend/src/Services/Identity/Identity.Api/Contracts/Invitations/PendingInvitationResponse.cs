namespace Identity.Api.Contracts.Invitations;

public sealed record PendingInvitationResponse(Guid InvitationId, string TenantName, string Role, DateTime ExpiresAt);