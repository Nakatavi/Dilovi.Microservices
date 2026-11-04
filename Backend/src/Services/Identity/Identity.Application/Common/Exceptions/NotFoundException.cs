namespace Identity.Application.Common.Exceptions;

public sealed class NotFoundException(Error error) : Exception(error.Message)
{
    public Error Error { get; } = error;
}