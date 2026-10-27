
namespace Identity.Application.Features.SuperAdmin.SeedSuperAdmin;

public sealed record SeedSuperAdminCommand(string Email, string Password) : IRequest;