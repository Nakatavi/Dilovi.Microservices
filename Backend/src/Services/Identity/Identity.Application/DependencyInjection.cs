using Identity.Application.Features.Auth;
using Identity.Application.Features.Invitations;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddScoped<SessionIssuer>();
        services.AddScoped<InvitationAcceptor>();
        services.AddScoped<InvitationIssuer>();
        return services;
    }
}