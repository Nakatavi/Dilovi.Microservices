namespace Identity.Domain.Entities;

public sealed class User : Entity
{
    public string Email { get; private set; } = null!;
    public string? PasswordHash { get; private set; }
    public bool IsSuperAdmin { get; private set; }
    public UserStatus Status { get; private set; }
    public Guid SecurityStamp { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool HasPassword => PasswordHash is not null;

    private User() { }

    public static User CreateInvited(string email, DateTime createdAt)
    => Create(email, passwordHash: null, isSuperAdmin: false, createdAt);

    public static User CreateSuperAdmin(string email, string passwordHash, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(DomainErrors.User.PasswordRequired);

        return Create(email, passwordHash, isSuperAdmin: true, createdAt);
    }

    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(DomainErrors.User.PasswordRequired);

        PasswordHash = passwordHash;
        RotateSecurityStamp();
    }

    public void RotateSecurityStamp() => SecurityStamp = Guid.NewGuid();

    private static User Create(string email, string? passwordHash, bool isSuperAdmin, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException(DomainErrors.User.InvalidEmail);

        return new User
        {
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            IsSuperAdmin = isSuperAdmin,
            Status = UserStatus.Active,
            SecurityStamp = Guid.NewGuid(),
            CreatedAt = createdAt
        };
    }

    

    public void SetInitialPassword(string passwordHash)
    {
        if (HasPassword)
            throw new DomainException(DomainErrors.User.PasswordAlreadySet);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(DomainErrors.User.PasswordRequired);

        PasswordHash = passwordHash;
    }
}