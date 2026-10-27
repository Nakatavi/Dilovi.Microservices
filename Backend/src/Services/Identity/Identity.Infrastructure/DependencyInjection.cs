using Identity.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDb")
            ?? throw new InvalidOperationException("Connection string 'IdentityDb' is not configured.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("Identity.Migrations")));

        return services;
    }
}