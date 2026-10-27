using Mediator;

namespace Identity.Api.Extensions;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediatorHandlers(this IServiceCollection services)
    {
        services.AddMediator((MediatorOptions options) =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.Assemblies = [typeof(Identity.Application.DependencyInjection)];
        });

        return services;
    }
}