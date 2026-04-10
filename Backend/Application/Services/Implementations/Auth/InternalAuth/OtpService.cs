using System.Text.Json;
using Application.Services.Interfaces.Auth.InternalAuth;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.Implementations.Auth.InternalAuth;

public class OtpService<T>(
    IDistributedCache cache,
    IOtpStrategy<T> strategy
    ) : IOtpService<T> where T : class
{
    private readonly IDistributedCache _cache = cache;
    private readonly IOtpStrategy<T> _strategy = strategy;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task CacheAsync(T payload, string otp, CancellationToken cancellationToken)
    {
        var key = BuildKey(_strategy.KeyPrefix, otp);
        var value = JsonSerializer.Serialize(payload, JsonOptions);

        await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _strategy.Expiration
        }, cancellationToken);
    }

    public async Task<T> GetDataAsync(string otp, CancellationToken cancellationToken)
    {
        var key = BuildKey(_strategy.KeyPrefix, otp);
        var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(cachedValue))
        {
            return default!;
        }

        try
        {
            if (cachedValue.TrimStart().StartsWith('{'))
            {
                return JsonSerializer.Deserialize<T>(cachedValue, JsonOptions)!;
            }
        }
        catch (JsonException) { }

        return default!;
    }

    public async Task<bool> IsOtpValidAsync(T payload, string otp, CancellationToken cancellationToken)
    {
        var key = BuildKey(_strategy.KeyPrefix, otp);
        var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(cachedValue))
        {
            return false;
        }
        var payloadJson = JsonSerializer.Serialize(payload, JsonOptions);
        return string.Equals(payloadJson, cachedValue, StringComparison.Ordinal);
    }

    public async Task InvalidateAsync(string otp, CancellationToken cancellationToken)
    {
        var key = BuildKey(_strategy.KeyPrefix, otp);
        await _cache.RemoveAsync(key, cancellationToken);
    }

    private static string BuildKey(string prefix, string otp)
    {
        return $"{prefix}{otp}";
    }
}