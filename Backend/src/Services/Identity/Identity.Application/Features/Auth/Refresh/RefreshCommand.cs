namespace Identity.Application.Features.Auth.Refresh;

public sealed record RefreshCommand(string? RefreshToken) : IRequest<AuthTokens>;