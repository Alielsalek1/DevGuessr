using System.Net;
using Domain.Models.User;

namespace Application.Services.Interfaces.Auth;

public interface ILoginThrottlingService
{
    Task<bool> IsUserJailed(Guid userId, IPAddress ipAddress, CancellationToken cancellationToken);
    Task<int> GetUserLoginAttempts(User user, IPAddress ipAddress, CancellationToken cancellationToken);
    Task IncrementUserLoginAttempts(User user, IPAddress ipAddress, int newAttempts, CancellationToken cancellationToken);
    bool ShouldBeJailed(int attempts);
    Task JailUser(User user, IPAddress ipAddress, CancellationToken cancellationToken);
}