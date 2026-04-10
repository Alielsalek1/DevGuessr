namespace Application.Services.Interfaces.Auth.InternalAuth;

public interface IOtpService<T> where T : class
{
    Task CacheAsync(T payload, string otp, CancellationToken cancellationToken);
    Task<T> GetDataAsync(string otp, CancellationToken cancellationToken);
    Task<bool> IsOtpValidAsync(T payload, string otp, CancellationToken cancellationToken);
    Task InvalidateAsync(string otp, CancellationToken cancellationToken);
}