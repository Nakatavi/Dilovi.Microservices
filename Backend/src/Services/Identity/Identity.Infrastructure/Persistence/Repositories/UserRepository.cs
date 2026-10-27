
namespace Identity.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    public Task<bool> SuperAdminExistsAsync(CancellationToken cancellationToken)
    => context.Users.AnyAsync(u => u.IsSuperAdmin, cancellationToken);

    public void Add(User user) => context.Users.Add(user);
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    => context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    => context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    => context.Users.AnyAsync(u => u.Email == email, cancellationToken);
}