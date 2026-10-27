namespace Identity.Domain.Enums;

public enum AuditAction
{
    TenantCreated = 1,
    TenantBlocked = 2,
    TenantUnblocked = 3,
    InvitationSent = 4,
    InvitationCancelled = 5,
    InvitationAccepted = 6,
    UserDeactivated = 7
}