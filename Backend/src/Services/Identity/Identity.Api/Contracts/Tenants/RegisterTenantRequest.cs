namespace Identity.Api.Contracts.Tenants;

public sealed record RegisterTenantRequest(string Name, string ChairmanEmail);