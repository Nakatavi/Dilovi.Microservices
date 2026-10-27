
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
}