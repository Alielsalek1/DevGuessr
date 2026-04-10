namespace Domain.Exceptions;

public class DbException(string message) : Exception(message)
{
}