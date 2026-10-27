using Identity.Api.Extensions;
using Identity.Infrastructure;
using Identity.Application;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    Log.Information("Starting Identity.Api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilogLogging(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddApiDocumentation();
    builder.Services.AddApplication();

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddErrorHandling();

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    app.UseApiDocumentation();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    await app.SeedSuperAdminAsync();

    app.Run();

}
catch (Exception ex) when (ex is not HostAbortedException)
{

    Log.Fatal(ex, "Identity.Api terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
