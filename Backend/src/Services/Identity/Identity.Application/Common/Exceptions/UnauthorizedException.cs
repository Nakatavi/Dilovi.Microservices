using Identity.Domain.Common;

namespace Identity.Application.Common.Exceptions;

public sealed class UnauthorizedException(Error error) : Exception(error.Message)
{
    public Error Error { get; } = error;
}