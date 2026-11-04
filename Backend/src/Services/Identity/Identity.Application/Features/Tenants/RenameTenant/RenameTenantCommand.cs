namespace Identity.Application.Features.Tenants.RenameTenant;

public sealed record RenameTenantCommand(Guid TenantId, string Name) : IRequest;