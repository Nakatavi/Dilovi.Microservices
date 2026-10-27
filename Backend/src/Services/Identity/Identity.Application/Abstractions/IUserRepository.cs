
namespace Identity.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> SuperAdminExistsAsync(CancellationToken cancellationToken);
    void Add(User user);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
}