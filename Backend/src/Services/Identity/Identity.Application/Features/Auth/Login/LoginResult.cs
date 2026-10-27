namespace Identity.Application.Features.Auth.Login;

public sealed record LoginResult(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);