using System.Net;
using Application.DTOs.Auth;
using Application.Utils;
using Npgsql;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class ExternalAuthSecurityTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ExternalAuthUser_CannotUsePasswordLogin_WithoutPasswordReset()
    {
        // Arrange: Create external auth user (google_id set, password_hash null)
        var userId = Guid.NewGuid();
        var email = $"googleuser_{userId.ToString().Substring(0, 8)}@example.com";
        await CreateExternalAuthUserAsync(userId, email, "google_12345");

        var loginRequest = new LoginRequestDto
        {
            UsernameOrEmail = email,
            Password = "AnyPassword123!"
        };

        // Act
        var (response, content, _) = await LoginTestHelpers.PostLoginAsync<FailApiResponse>(Client, loginRequest);

        // Assert: External auth user blocked from password login
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
    }

    [Fact]
    public async Task ExternalAuthUser_AfterPasswordReset_CanUsePasswordLogin()
    {
        // Arrange: Create external auth user with password set (simulating password reset)
        var userId = Guid.NewGuid();
        var email = $"hybriduser_{userId.ToString().Substring(0, 8)}@example.com";
        var password = "HybridPassword123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        await CreateExternalAuthUserWithPasswordAsync(userId, email, "google_67890", passwordHash);

        var loginRequest = new LoginRequestDto
        {
            UsernameOrEmail = email,
            Password = password
        };

        // Act
        var (response, content, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);

        // Assert: Hybrid user can use password login
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.NotNull(content.Data.AccessToken);
    }

    [Fact]
    public async Task GoogleAuthUser_UsernameIsAlwaysNull()
    {
        // Arrange: Create Google auth user
        var userId = Guid.NewGuid();
        var email = $"nullusername_{userId.ToString().Substring(0, 8)}@example.com";
        await CreateExternalAuthUserAsync(userId, email, "google_nulltest");

        // Act: Verify in database
        var username = await GetUsernameFromDbAsync(userId);

        // Assert: Username is NULL for Google auth users
        Assert.Null(username);
    }

    private static async Task CreateExternalAuthUserAsync(Guid userId, string email, string googleId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO users (id, username, password_hash, email, is_email_verified, role, address, phone_number, google_id)
                            VALUES (@id, @username, @password_hash, @email, @is_email_verified, @role, @address, @phone_number, @google_id);";
        cmd.Parameters.AddWithValue("@id", userId);
        cmd.Parameters.AddWithValue("@username", DBNull.Value); // NULL username for Google auth
        cmd.Parameters.AddWithValue("@password_hash", DBNull.Value); // NULL password
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@is_email_verified", true);
        cmd.Parameters.AddWithValue("@role", 0);
        cmd.Parameters.AddWithValue("@address", string.Empty);
        cmd.Parameters.AddWithValue("@phone_number", string.Empty);
        cmd.Parameters.AddWithValue("@google_id", googleId);

        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task CreateExternalAuthUserWithPasswordAsync(Guid userId, string email, string googleId, string passwordHash)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO users (id, username, password_hash, email, is_email_verified, role, address, phone_number, google_id)
                            VALUES (@id, @username, @password_hash, @email, @is_email_verified, @role, @address, @phone_number, @google_id);";
        cmd.Parameters.AddWithValue("@id", userId);
        cmd.Parameters.AddWithValue("@username", DBNull.Value); // Still NULL even with password
        cmd.Parameters.AddWithValue("@password_hash", passwordHash);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@is_email_verified", true);
        cmd.Parameters.AddWithValue("@role", 0);
        cmd.Parameters.AddWithValue("@address", string.Empty);
        cmd.Parameters.AddWithValue("@phone_number", string.Empty);
        cmd.Parameters.AddWithValue("@google_id", googleId);

        await cmd.ExecuteNonQueryAsync();

        // Seed device to avoid device confirmation email
        await AuthBackdoor.SeedUserDeviceAsync(email, AuthBackdoor.TestDeviceId);
    }

    private static async Task<string?> GetUsernameFromDbAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT username FROM users WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", userId);

        var result = await cmd.ExecuteScalarAsync();
        return result == DBNull.Value ? null : (string?)result;
    }
}
