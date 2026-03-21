using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TestsReusables.Auth;

public static class AuthBackdoor
{
    public static readonly Guid TestDeviceId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Inserts a registered user into the test Postgres database with IsEmailVerified = true.
    /// Also seeds the default test device for this user to ensure 200 OK on login.
    /// Returns the created user's Guid and the plain password used.
    /// </summary>
    public static async Task<(Guid UserId, string Password, string Username, string Email)> CreateVerifiedUserAsync(string? username = null, string? email = null, string? password = null)
    {
        var result = await CreateUserAsync(true, username, email, password);
        await SeedUserDeviceAsync(result.Email, TestDeviceId);
        return result;
    }

    /// <summary>
    /// Inserts a registered user into the test Postgres database with IsEmailVerified = false.
    /// Returns the created user's Guid and the plain password used.
    /// </summary>
    public static async Task<(Guid UserId, string Password, string Username, string Email)> CreateUnverifiedUserAsync(string? username = null, string? email = null, string? password = null)
    {
        return await CreateUserAsync(false, username, email, password);
    }

    private static async Task<(Guid UserId, string Password, string Username, string Email)> CreateUserAsync(bool isEmailVerified, string? username, string? email, string? password)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        var userId = Guid.NewGuid();
        var pwd = password ?? "TestPassword123";
        var uname = username ?? ($"user_{userId.ToString().Substring(0, 8)}");
        var mail = email ?? ($"{uname}@example.com");

        // Hash the password using BCrypt (Application project includes BCrypt dependency)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(pwd, BCrypt.Net.BCrypt.GenerateSalt());

        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO users (id, username, password_hash, email, is_email_verified, role, address, phone_number, google_id)
                            VALUES (@id, @username, @password_hash, @email, @is_email_verified, @role, @address, @phone_number, @google_id);";
        cmd.Parameters.AddWithValue("@id", userId);
        cmd.Parameters.AddWithValue("@username", uname);
        cmd.Parameters.AddWithValue("@password_hash", passwordHash);
        cmd.Parameters.AddWithValue("@email", mail);
        cmd.Parameters.AddWithValue("@is_email_verified", isEmailVerified);
        cmd.Parameters.AddWithValue("@role", 0);
        cmd.Parameters.AddWithValue("@address", string.Empty);
        cmd.Parameters.AddWithValue("@phone_number", string.Empty);
        cmd.Parameters.AddWithValue("@google_id", DBNull.Value);

        await cmd.ExecuteNonQueryAsync();

        return (userId, pwd, uname, mail);
    }

    /// <summary>
    /// Helper to seed a confirmation token into the Redis cache used by the application.
    /// Uses the app's IDistributedCache to ensure correct formatting (avoids WRONGTYPE errors).
    /// The token is stored with the "new_user:" prefix to match the application's convention.
    /// </summary>
    public static async Task SeedConfirmationTokenAsync(Tests.Common.CustomWebApplicationFactory factory, Guid userId, string token, int ttlSeconds = 600)
    {
        using var scope = factory.Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        var key = $"new_user:{token}";
        var json = JsonSerializer.Serialize(new Application.DTOs.Auth.InternalAuth.RegistrationOtpPayload(userId), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
        });
    }

    /// <summary>
    /// Helper to seed a password reset token into the Redis cache used by the application.
    /// The token is stored with the "reset_password:" prefix to match the application's convention.
    /// </summary>
    public static async Task SeedPasswordResetTokenAsync(Tests.Common.CustomWebApplicationFactory factory, Guid userId, string token, int ttlSeconds = 600)
    {
        using var scope = factory.Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        var key = $"reset_password:{token}";
        var json = JsonSerializer.Serialize(new Application.DTOs.Auth.InternalAuth.PasswordResetOtpPayload(userId), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
        });
    }

    /// <summary>
    /// Helper to seed a new device login token into the Redis cache used by the application.
    /// The token is stored with the "new_device:" prefix and value "{userId}:{deviceId}".
    /// </summary>
    public static async Task SeedNewDeviceTokenAsync(Tests.Common.CustomWebApplicationFactory factory, Guid userId, Guid deviceId, string token, int ttlSeconds = 600)
    {
        using var scope = factory.Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        var key = $"new_device:{token}";
        var json = JsonSerializer.Serialize(new Application.DTOs.Auth.InternalAuth.NewDeviceOtpPayload(userId, deviceId), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
        });
    }

    public static async Task SeedUserDeviceAsync(string email, Guid deviceId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        // Get user ID
        Guid userId;
        var getUserIdCmd = conn.CreateCommand();
        getUserIdCmd.CommandText = "SELECT id FROM users WHERE email = @email";
        getUserIdCmd.Parameters.AddWithValue("@email", email);
        var result = await getUserIdCmd.ExecuteScalarAsync();
        if (result == null) throw new Exception("User not found: " + email);
        userId = (Guid)result;

        // Insert device
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO user_devices (user_id, device_id) VALUES (@user_id, @device_id) ON CONFLICT DO NOTHING";
        cmd.Parameters.AddWithValue("@user_id", userId);
        cmd.Parameters.AddWithValue("@device_id", deviceId);
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task SeedUserDeviceAsync(Guid userId, Guid deviceId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO user_devices (user_id, device_id) VALUES (@user_id, @device_id) ON CONFLICT DO NOTHING";
        cmd.Parameters.AddWithValue("@user_id", userId);
        cmd.Parameters.AddWithValue("@device_id", deviceId);
        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Creates a Google OAuth user (external auth) without password.
    /// Returns userId, username, and email.
    /// </summary>
    public static async Task<(Guid UserId, string Username, string Email)> CreateExternalAuthUserAsync(string? username = null, string? email = null)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        var userId = Guid.NewGuid();
        var uname = username ?? ($"googleuser_{userId.ToString().Substring(0, 8)}");
        var mail = email ?? ($"{uname}@gmail.com");
        var googleId = $"google_{Guid.NewGuid().ToString().Substring(0, 16)}";

        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO users (id, username, password_hash, email, is_email_verified, role, address, phone_number, google_id)
                            VALUES (@id, @username, @password_hash, @email, @is_email_verified, @role, @address, @phone_number, @google_id);";
        cmd.Parameters.AddWithValue("@id", userId);
        cmd.Parameters.AddWithValue("@username", uname);
        cmd.Parameters.AddWithValue("@password_hash", DBNull.Value);
        cmd.Parameters.AddWithValue("@email", mail);
        cmd.Parameters.AddWithValue("@is_email_verified", true); // Google users are pre-verified
        cmd.Parameters.AddWithValue("@role", 0);
        cmd.Parameters.AddWithValue("@address", string.Empty);
        cmd.Parameters.AddWithValue("@phone_number", string.Empty);
        cmd.Parameters.AddWithValue("@google_id", googleId);

        await cmd.ExecuteNonQueryAsync();

        return (userId, uname, mail);
    }

    /// <summary>
    /// Adds a refresh token for a user in the user_refresh_tokens table.
    /// Returns the plain refresh token string.
    /// </summary>
    public static async Task<string> AddRefreshTokenAsync(Guid userId, DateTime? expiryTime = null)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        var refreshToken = GenerateNewRefreshToken();
        var expiry = expiryTime ?? DateTime.UtcNow.AddDays(30);

        // Hash the refresh token using SHA256 (matching application logic)
        var refreshTokenHash = HashRefreshToken(refreshToken);

        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO user_refresh_tokens (user_id, refresh_token_hash, is_used, used_at, refresh_token_expiry_time)
                            VALUES (@user_id, @refresh_token_hash, @is_used, @used_at, @refresh_token_expiry_time);";
        cmd.Parameters.AddWithValue("@user_id", userId);
        cmd.Parameters.AddWithValue("@refresh_token_hash", refreshTokenHash);
        cmd.Parameters.AddWithValue("@is_used", false);
        cmd.Parameters.AddWithValue("@used_at", DBNull.Value);
        cmd.Parameters.AddWithValue("@refresh_token_expiry_time", expiry);

        await cmd.ExecuteNonQueryAsync();

        return refreshToken;
    }

    // Helper to match the service's refresh token generation
    private static string GenerateNewRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Gets the count of refresh tokens for a user.
    /// </summary>
    public static async Task<int> GetRefreshTokenCountAsync(Guid userId, bool onlyActive = true)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        if (onlyActive)
        {
            cmd.CommandText = "SELECT COUNT(*) FROM user_refresh_tokens WHERE user_id = @user_id AND is_used = false";
        }
        else
        {
            cmd.CommandText = "SELECT COUNT(*) FROM user_refresh_tokens WHERE user_id = @user_id";
        }
        cmd.Parameters.AddWithValue("@user_id", userId);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    private static string HashRefreshToken(string refreshToken)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
        var hash = sha256.ComputeHash(bytes);
        // Must match the format used in InternalSessionService: lowercase hex string
        return Convert.ToHexString(hash).ToLower();
    }
}
