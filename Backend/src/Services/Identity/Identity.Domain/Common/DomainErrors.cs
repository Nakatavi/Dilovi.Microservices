
namespace Identity.Domain.Common;

public static class DomainErrors
{
    public static class Tenant
    {
        public static readonly Error NameRequired = new("Tenant.NameRequired", "Tenant name is required.");
    }
    public static class User
    {
        public static readonly Error InvalidEmail = new("User.InvalidEmail", "A valid email is required.");
        public static readonly Error PasswordRequired = new("User.PasswordRequired", "Password is required.");
    }
    public static class Invitation
    {
        public static readonly Error InvalidEmail = new("Invitation.InvalidEmail", "A valid email is required.");
        public static readonly Error NotPending = new("Invitation.NotPending", "Invitation has already been used or cancelled.");
        public static readonly Error Expired = new("Invitation.Expired", "Invitation has expired.");
    }
}
