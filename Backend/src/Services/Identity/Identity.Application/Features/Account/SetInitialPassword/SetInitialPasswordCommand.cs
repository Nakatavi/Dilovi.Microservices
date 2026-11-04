namespace Identity.Application.Features.Account.SetInitialPassword;

public sealed record SetInitialPasswordCommand(string Password) : IRequest;
