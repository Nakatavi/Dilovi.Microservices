
namespace Identity.Application.Abstractions;

public interface ITokenService
{
    AccessToken CreateAccessToken(User user);
    GeneratedRefreshToken GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}

public sealed record AccessToken(string Token, DateTime ExpiresAt);

public sealed record GeneratedRefreshToken(string Token, string TokenHash);