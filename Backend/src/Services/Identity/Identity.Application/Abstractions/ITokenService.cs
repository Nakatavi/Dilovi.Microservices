
namespace Identity.Application.Abstractions;

public interface ITokenService
{
    AccessToken CreateAccessToken(User user, AvailableMembership? membership);
    GeneratedToken GenerateOpaqueToken();
    string HashOpaqueToken(string opaqueToken);
}

public sealed record AccessToken(string Token, DateTime ExpiresAt);

public sealed record GeneratedToken(string Token, string TokenHash);