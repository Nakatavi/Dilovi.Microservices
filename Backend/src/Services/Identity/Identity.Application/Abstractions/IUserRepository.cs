
namespace Identity.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> SuperAdminExistsAsync(CancellationToken cancellationToken);
    void Add(User user);
}