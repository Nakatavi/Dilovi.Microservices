namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class RefreshTokenRepository(IdentityDbContext context) : IRefreshTokenRepository
{
    public void Add(RefreshToken refreshToken) => context.RefreshTokens.Add(refreshToken);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken)
        => context.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);

    public Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, DateTime now, CancellationToken cancellationToken)
        => context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > now)
            .ToListAsync(cancellationToken);
}