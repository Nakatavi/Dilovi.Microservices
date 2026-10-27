
namespace Identity.Api.Infrastructure;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, error) = exception switch
        {
            DomainException e => (StatusCodes.Status422UnprocessableEntity, e.Error),
            ConflictException e => (StatusCodes.Status409Conflict, e.Error),
            UnauthorizedException e => (StatusCodes.Status401Unauthorized, e.Error),
            _ => (StatusCodes.Status500InternalServerError, new Error("Server.Error", "An unexpected error occurred."))
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = status;

        var problem = new ProblemDetails { Status = status, Title = error.Code, Detail = error.Message };
        problem.Extensions["code"] = error.Code;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }
}