namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NotificationDb")
            ?? throw new InvalidOperationException("Connection string 'NotificationDb' is not configured.");

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("Notification.Migrations")));

        services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}