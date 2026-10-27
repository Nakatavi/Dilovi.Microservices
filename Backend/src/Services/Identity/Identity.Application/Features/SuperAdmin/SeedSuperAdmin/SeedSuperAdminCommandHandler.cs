
namespace Identity.Application.Features.SuperAdmin.SeedSuperAdmin;

public sealed class SeedSuperAdminCommandHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider) : IRequestHandler<SeedSuperAdminCommand>
{
    public async Task Handle(SeedSuperAdminCommand command, CancellationToken cancellationToken)
    {
        if (await users.SuperAdminExistsAsync(cancellationToken))
            return;

        var superAdmin = User.CreateSuperAdmin(
            command.Email,
            passwordHasher.Hash(command.Password),
            timeProvider.GetUtcNow().UtcDateTime);

        users.Add(superAdmin);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return;
    }
}