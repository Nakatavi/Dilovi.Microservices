namespace Identity.Application.Features.Account.SetInitialPassword;

public sealed class SetInitialPasswordCommandHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<SetInitialPasswordCommand>
{
    public async Task Handle(SetInitialPasswordCommand command, CancellationToken cancellationToken)
    {
        if (!PasswordPolicy.IsSatisfiedBy(command.Password))
            throw new ValidationException(ApplicationErrors.Passwords.TooWeak);

        var user = await users.GetByIdAsync(currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationErrors.Invitations.NotFound);

        user.SetInitialPassword(passwordHasher.Hash(command.Password));
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
