namespace Identity.Application.Common.Exceptions;

public sealed class ConflictException(Error error) : Exception(error.Message)
{
    public Error Error { get; } = error;
}