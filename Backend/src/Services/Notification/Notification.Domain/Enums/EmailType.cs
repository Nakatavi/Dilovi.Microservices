namespace Notification.Domain.Enums;

public enum EmailType
{
    InvitationNewUser = 1,
    InvitationExistingUser = 2,
    InvitationCancelled = 3,
    InvitationAccepted = 4,
    InvitationDeclined = 5,
    PasswordReset = 6,
    PasswordChanged = 7,
    EmailChangeConfirmation = 8,
    EmailChanged = 9,
    MembershipAccessRevoked = 10,
    TenantBlocked = 11,
    TenantUnblocked = 12
}
