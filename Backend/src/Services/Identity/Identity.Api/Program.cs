using Identity.Application;
using Identity.Infrastructure;
using Serilog;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    Log.Information("Starting Identity.Api");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilogLogging(builder.Configuration);

    builder.Services.AddControllers()
     .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    builder.Services.AddApiDocumentation();
    builder.Services.AddApplication();
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddErrorHandling();


    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();
    app.UseApiDocumentation();
    app.UseHttpsRedirection();
    app.UseAuthentication();
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
