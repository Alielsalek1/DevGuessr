using System.Net;
using Application.DTOs.User;
using Application.Utils;
using Npgsql;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class RefreshTokenSecurityTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RefreshToken_IsStoredAsLowercaseHexSHA256()
    {
        // Arrange & Act: Create user and add refresh token
        var (userId, password, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("SHA256User", "sha256test@example.com", "Password123!");
        var refreshToken = await AuthBackdoor.AddRefreshTokenAsync(userId);

        // Assert: Check token in database
        var storedHash = await GetRefreshTokenHashFromDbAsync(userId);
        Assert.NotNull(storedHash);
        Assert.Equal(64, storedHash.Length); // SHA256 hex is 64 characters
        Assert.Matches("^[a-f0-9]{64}$", storedHash); // Lowercase hex only
    }

    [Fact]
    public async Task RefreshToken_RotatesOnRefresh()
    {
        // Arrange: Create user with refresh token
        var (userId, password, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("RotateUser", "rotate@example.com", "Password123!");
        var refreshToken = await AuthBackdoor.AddRefreshTokenAsync(userId);
        var originalHash = await GetRefreshTokenHashFromDbAsync(userId);

        // Act: Refresh the token
        var refreshRequest = new RefreshTokenRequestDto { UserId = userId };
        var (refreshResponse, refreshContent, _, newRefreshToken) = await RefreshTokenTestHelpers.PostRefreshTokenAsync<SuccessApiResponse<RefreshTokenResponseDto>>(Client, refreshRequest, refreshToken);
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var newHash = await GetRefreshTokenHashFromDbAsync(userId);

        // Assert: New token hash is different and old token is marked as used
        Assert.NotEqual(originalHash, newHash);

        var isOldTokenUsed = await IsRefreshTokenMarkedAsUsedAsync(originalHash!);
        Assert.True(isOldTokenUsed);
    }

    [Fact]
    public async Task MultipleDevices_HaveSeparateRefreshTokens()
    {
        // Arrange: Create user and add tokens for 2 devices
        var (userId, password, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("MultiDeviceUser", "multidevice@example.com", "Password123!");

        var device1 = Guid.NewGuid();
        var device2 = Guid.NewGuid();

        await AuthBackdoor.SeedUserDeviceAsync(email, device1);
        await AuthBackdoor.SeedUserDeviceAsync(email, device2);

        await AuthBackdoor.AddRefreshTokenAsync(userId);
        await AuthBackdoor.AddRefreshTokenAsync(userId);

        // Assert: Both tokens exist in database
        var tokenCount = await GetRefreshTokenCountForUserAsync(userId);
        Assert.True(tokenCount >= 2); // At least 2 tokens
    }

    private static async Task<string?> GetRefreshTokenHashFromDbAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT refresh_token_hash FROM user_refresh_tokens WHERE user_id = @userId AND is_used = false LIMIT 1;";
        cmd.Parameters.AddWithValue("@userId", userId);

        var result = await cmd.ExecuteScalarAsync();
        return result as string;
    }

    private static async Task<bool> IsRefreshTokenMarkedAsUsedAsync(string tokenHash)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT is_used FROM user_refresh_tokens WHERE refresh_token_hash = @hash;";
        cmd.Parameters.AddWithValue("@hash", tokenHash);

        var result = await cmd.ExecuteScalarAsync();
        return result != null && (bool)result;
    }

    private static async Task<int> GetRefreshTokenCountForUserAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM user_refresh_tokens WHERE user_id = @userId;";
        cmd.Parameters.AddWithValue("@userId", userId);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
}
