using Identity.Application.Features.SuperAdmin.SeedSuperAdmin;

namespace Identity.Api.Extensions;

public static class SeedExtensions
{
    public static async Task SeedSuperAdminAsync(this WebApplication app)
    {
        var email = app.Configuration["SuperAdmin:Email"];
        var password = app.Configuration["SuperAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            app.Logger.LogWarning("SuperAdmin seed skipped: email or password is not configured.");
            return;
        }

        using var scope = app.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new SeedSuperAdminCommand(email, password));
    }
}