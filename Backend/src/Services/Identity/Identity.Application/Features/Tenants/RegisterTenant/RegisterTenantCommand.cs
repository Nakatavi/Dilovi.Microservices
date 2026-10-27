namespace Identity.Application.Features.Tenants.RegisterTenant;

public sealed record RegisterTenantCommand(string Name, string ChairmanEmail) : IRequest<Guid>;