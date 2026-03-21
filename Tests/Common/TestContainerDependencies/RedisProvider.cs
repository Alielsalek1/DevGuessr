using Testcontainers.Redis;

namespace Tests.Common.TestContainerDependencies;

public class RedisProvider
{
    private readonly RedisContainer _container;

    public RedisProvider(RedisContainer container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public async Task FlushAllAsync()
    {
        await _container.ExecAsync(new[] { "redis-cli", "FLUSHALL" });
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var result = await _container.ExecAsync(new[] { "redis-cli", "GET", key });
        var output = result.Stdout.Trim();
        return string.IsNullOrEmpty(output) || output == "(nil)" ? null : output;
    }

    /// <summary>
    /// Gets the value of a string key from Redis.
    /// Returns null if the key doesn't exist.
    /// </summary>
    public async Task<string?> GetStringAsync(string key)
    {
        return await GetValueAsync(key);
    }

    /// <summary>
    /// Sets a string value in Redis with an optional expiry.
    /// </summary>
    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        if (expiry.HasValue)
        {
            var ttlSeconds = (int)expiry.Value.TotalSeconds;
            await _container.ExecAsync(new[] { "redis-cli", "SET", key, value, "EX", ttlSeconds.ToString() });
        }
        else
        {
            await _container.ExecAsync(new[] { "redis-cli", "SET", key, value });
        }
    }

    /// <summary>
    /// Deletes a key from Redis.
    /// </summary>
    public async Task DeleteKeyAsync(string key)
    {
        await _container.ExecAsync(new[] { "redis-cli", "DEL", key });
    }

    /// <summary>
    /// Gets the time-to-live for a key in Redis.
    /// Returns null if the key doesn't exist or has no expiry.
    /// </summary>
    public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
    {
        var result = await _container.ExecAsync(new[] { "redis-cli", "TTL", key });
        if (long.TryParse(result.Stdout.Trim(), out var ttlSeconds))
        {
            if (ttlSeconds == -2) return null; // Key doesn't exist
            if (ttlSeconds == -1) return TimeSpan.MaxValue; // Key exists but has no expiry
            return TimeSpan.FromSeconds(ttlSeconds);
        }
        return null;
    }

    public async Task<long> GetTTLAsync(string key)
    {
        var result = await _container.ExecAsync(new[] { "redis-cli", "TTL", key });
        if (long.TryParse(result.Stdout.Trim(), out var ttl))
        {
            return ttl;
        }
        return -2;
    }

    public async Task SetValueAsync(string key, string value, int ttlSeconds)
    {
        await _container.ExecAsync(new[] { "redis-cli", "SET", key, value, "EX", ttlSeconds.ToString() });
    }
}
