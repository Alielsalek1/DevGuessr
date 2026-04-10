using System.Net;
using Application.Services.Interfaces.Auth;
using Domain.Models.User;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.Implementations.Misc;

public class LoginThrottlingService(IDistributedCache cache) : ILoginThrottlingService
{
    private readonly IDistributedCache _cache = cache;

    public async Task<bool> IsUserJailed(Guid userId, IPAddress ipAddress, CancellationToken cancellationToken)
    {
        return await _cache.GetStringAsync($"jail:{userId}:{ipAddress}", cancellationToken) == "true";
    }

    public async Task<int> GetUserLoginAttempts(User user, IPAddress ipAddress, CancellationToken cancellationToken)
    {
        string key = $"login_attempts:{user.Id}:{ipAddress}";
        var attemptsString = await _cache.GetStringAsync(key, cancellationToken);
        return string.IsNullOrEmpty(attemptsString) ? 0 : int.Parse(attemptsString);
    }

    public async Task IncrementUserLoginAttempts(User user, IPAddress ipAddress, int newAttempts, CancellationToken cancellationToken)
    {
        string key = $"login_attempts:{user.Id}:{ipAddress}";
        await _cache.SetStringAsync(key, newAttempts.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
        }, cancellationToken);
    }

    public bool ShouldBeJailed(int attempts)
    {
        return attempts > 3;
    }

    public async Task JailUser(User user, IPAddress ipAddress, CancellationToken cancellationToken)
    {
        await _cache.SetStringAsync($"jail:{user!.Id}:{ipAddress}", "true", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        }, cancellationToken);
    }
}