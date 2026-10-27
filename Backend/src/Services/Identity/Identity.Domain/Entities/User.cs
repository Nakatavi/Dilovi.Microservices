namespace Identity.Domain.Entities;

public sealed class User : Entity
{
    public Guid? TenantId { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public string? Email { get; private set; }
    public string? PasswordHash { get; private set; }
    public Guid SecurityStamp { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    private User() { }

    public static User CreateSuperAdmin(string email, string passwordHash, DateTime createdAt)
    {
        var user = new User { Role = UserRole.SuperAdmin, CreatedAt = createdAt };
        user.Activate(email, passwordHash);
        return user;
    }

    public static User CreateForTenant(Guid tenantId, UserRole role, DateTime createdAt)
    {
        if (role == UserRole.SuperAdmin)
            throw new DomainException(DomainErrors.User.SuperAdminCannotBelongToTenant);

        return new User
        {
            TenantId = tenantId,
            Role = role,
            Status = UserStatus.PendingActivation,
            CreatedAt = createdAt
        };
    }

    public void Activate(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException(DomainErrors.User.InvalidEmail);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(DomainErrors.User.PasswordRequired);

        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Status = UserStatus.Active;
        DeactivatedAt = null;
        SecurityStamp = Guid.NewGuid();
    }

    public void Deactivate(DateTime deactivatedAt)
    {
        if (Status == UserStatus.Deactivated)
            return;

        Status = UserStatus.Deactivated;
        DeactivatedAt = deactivatedAt;
        SecurityStamp = Guid.NewGuid();
    }
}
