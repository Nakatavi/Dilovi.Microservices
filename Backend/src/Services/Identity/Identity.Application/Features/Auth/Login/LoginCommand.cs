namespace Identity.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password, Guid? MembershipId = null) : IRequest<AuthTokens>;