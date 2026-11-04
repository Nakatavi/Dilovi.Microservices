namespace Identity.Application.Common.Exceptions;

public sealed class ForbiddenException(Error error) : Exception(error.Message)
{
    public Error Error { get; } = error;
}