public sealed record AuthTokens(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    Guid? CurrentMembershipId,
    IReadOnlyList<AvailableMembership> Memberships);