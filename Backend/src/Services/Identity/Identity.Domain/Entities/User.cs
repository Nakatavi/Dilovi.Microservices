namespace Identity.Domain.Entities;

public sealed class User : Entity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsSuperAdmin { get; private set; }
    public UserStatus Status { get; private set; }
    public Guid SecurityStamp { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static User Create(string email, string passwordHash, DateTime createdAt)
        => Create(email, passwordHash, isSuperAdmin: false, createdAt);

    public static User CreateSuperAdmin(string email, string passwordHash, DateTime createdAt)
        => Create(email, passwordHash, isSuperAdmin: true, createdAt);

    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(DomainErrors.User.PasswordRequired);

        PasswordHash = passwordHash;
        RotateSecurityStamp();
    }

    public void RotateSecurityStamp() => SecurityStamp = Guid.NewGuid();

    private static User Create(string email, string passwordHash, bool isSuperAdmin, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException(DomainErrors.User.InvalidEmail);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(DomainErrors.User.PasswordRequired);

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
}