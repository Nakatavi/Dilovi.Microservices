namespace Identity.Domain.Common;

public class DomainException(Error error) : Exception(error.Message)
{
    public Error Error { get; } = error;
}
