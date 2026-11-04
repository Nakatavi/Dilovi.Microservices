
namespace Identity.Application.Common;

public static class ApplicationErrors
{
    public static class Auth
    {
        public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid email or password.");
        public static readonly Error SessionEnded = new("Auth.SessionEnded", "Session has ended. Please sign in again.");
        public static readonly Error InvalidRefreshToken = new("Auth.InvalidRefreshToken", "Refresh token is invalid or expired.");
    }
    public static class Users
    {
        public static readonly Error EmailAlreadyUsed = new("Users.EmailAlreadyUsed", "This email is already used by another account.");
    }
    public static class Invitations
    {
        public static readonly Error NotFound = new("Invitations.NotFound", "Invitation was not found.");
        public static readonly Error LoginRequired = new("Invitations.LoginRequired", "An account with this email already exists. Sign in to accept the invitation.");
    }

    public static class Passwords
    {
        public static readonly Error TooWeak = new("Passwords.TooWeak",
            "Password must be at least 6 characters and contain an uppercase letter, a lowercase letter, a digit and a special character.");
    }

    public static class Positions
    {
        public static readonly Error AlreadyExists = new("Positions.AlreadyExists", "This position already exists in the tenant.");
        public static readonly Error ChairmanNotAllowed = new("Positions.ChairmanNotAllowed", "The chairman position is created together with the tenant.");
        public static readonly Error TenantRequired = new("Positions.TenantRequired", "Select a tenant first.");
        public static readonly Error NotFound = new("Positions.NotFound", "Position was not found.");
        public static readonly Error Forbidden = new("Positions.Forbidden", "You are not allowed to manage this position.");
        public static readonly Error AlreadyHeld = new("Positions.AlreadyHeld", "This person already holds the position.");
    }

    public static class Tenants
    {
        public static readonly Error NotFound = new("Tenants.NotFound", "Tenant was not found.");
        public static readonly Error Forbidden = new("Tenants.Forbidden", "You are not allowed to access this tenant.");
    }
}