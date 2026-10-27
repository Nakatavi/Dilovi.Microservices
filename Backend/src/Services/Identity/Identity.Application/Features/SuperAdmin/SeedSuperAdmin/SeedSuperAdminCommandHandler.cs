
namespace Identity.Application.Features.SuperAdmin.SeedSuperAdmin;

public sealed class SeedSuperAdminCommandHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    TimeProvider timeProvider) : ICommandHandler<SeedSuperAdminCommand>
{
    public async ValueTask<Unit> Handle(SeedSuperAdminCommand command, CancellationToken cancellationToken)
    {
        if (await users.SuperAdminExistsAsync(cancellationToken))
            return Unit.Value;

        var superAdmin = User.CreateSuperAdmin(
            command.Email,
            passwordHasher.Hash(command.Password),
            timeProvider.GetUtcNow().UtcDateTime);

        users.Add(superAdmin);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}