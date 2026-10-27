
namespace Identity.Api.Infrastructure;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId => Guid.Parse(GetClaim("sub"));
    public string Email => GetClaim("email");

    private string GetClaim(string type)
        => httpContextAccessor.HttpContext?.User.FindFirst(type)?.Value
           ?? throw new InvalidOperationException($"Claim '{type}' is not available for the current request.");
}