namespace Identity.Api.Infrastructure;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? Role => FindClaim("role");
    public Guid UserId => Guid.Parse(GetRequiredClaim("sub"));
    public string Email => GetRequiredClaim("email");
    public Guid? TenantId => GetOptionalGuid("tenant_id");
    public Guid? MembershipId => GetOptionalGuid("membership_id");

    private string GetRequiredClaim(string type)
        => FindClaim(type) ?? throw new InvalidOperationException($"Claim '{type}' is not available for the current request.");

    private Guid? GetOptionalGuid(string type)
        => Guid.TryParse(FindClaim(type), out var value) ? value : null;

    private string? FindClaim(string type)
        => httpContextAccessor.HttpContext?.User.FindFirst(type)?.Value;
}