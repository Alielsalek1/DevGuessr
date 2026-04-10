namespace Application.Services.Interfaces.Auth.InternalAuth;

public interface IOtpStrategy<T>
{
    string KeyPrefix { get; }
    TimeSpan Expiration { get; }
}