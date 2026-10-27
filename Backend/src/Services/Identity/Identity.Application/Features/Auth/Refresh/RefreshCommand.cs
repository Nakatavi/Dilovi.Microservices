namespace Identity.Application.Features.Auth.Refresh;

public sealed record RefreshCommand(string? RefreshToken, Guid? MembershipId = null) : IRequest<AuthTokens>;