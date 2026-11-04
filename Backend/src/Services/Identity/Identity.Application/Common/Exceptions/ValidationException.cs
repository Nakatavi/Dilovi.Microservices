namespace Identity.Application.Common.Exceptions;

public sealed class ValidationException(Error error) : Exception(error.Message)
{
    public Error Error { get; } = error;
}