namespace Domain.Exceptions;

public class DomainException(string message, string parameterName) : Exception(message)
{
    public DomainException(string message) : this(message, string.Empty)
    {
    }
}
