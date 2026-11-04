namespace Identity.Application.Common;

public static class PasswordPolicy
{
    public static bool IsSatisfiedBy(string password)
        => password.Length >= 6
           && password.Any(char.IsUpper)
           && password.Any(char.IsLower)
           && password.Any(char.IsDigit)
           && password.Any(c => !char.IsLetterOrDigit(c));
}