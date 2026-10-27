
namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    public Task<bool> SuperAdminExistsAsync(CancellationToken cancellationToken)
        => context.Users.AnyAsync(u => u.Role == UserRole.SuperAdmin, cancellationToken);

    public void Add(User user) => context.Users.Add(user);
}