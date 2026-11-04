namespace Identity.Application.Common;

public static class Permissions
{
    public static bool IsSuperAdmin(ICurrentUser user) => user.Role == "SuperAdmin";

    public static bool IsChairmanOf(ICurrentUser user, Guid tenantId)
        => user.Role == nameof(MembershipRole.Chairman) && user.TenantId == tenantId;

    public static bool CanViewTenant(ICurrentUser user, Guid tenantId)
        => IsSuperAdmin(user) || user.TenantId == tenantId;

    public static bool CanInvite(ICurrentUser user, Membership position)
        => (IsSuperAdmin(user) && position.Role == MembershipRole.Chairman) || IsChairmanOf(user, position.TenantId);

    public static bool CanDeactivate(ICurrentUser user, Membership position)
        => IsSuperAdmin(user) || (IsChairmanOf(user, position.TenantId) && position.Role != MembershipRole.Chairman);
}