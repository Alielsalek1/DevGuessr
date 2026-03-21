using System.Net;
using Application.DTOs.Auth;
using Application.Utils;
using Tests.Common;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class GuestPromoteSuccessTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GuestPromote_WithValidData_Returns201CreatedAndSendsEmail()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "PromotedGuest1",
            Email = "promotedguest1@example.com",
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<SuccessApiResponse<RegisterResponseDto>>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal(201, content.StatusCode);
        Assert.Contains("registered successfully", content.Message.ToLower());
        Assert.Contains("confirmation code", content.Message.ToLower());

        // Verify the userId remains the same
        Assert.Equal(guestUserId, content.Data.UserId);
    }

    [Fact]
    public async Task GuestPromote_UpdatesUserRecordInDatabase()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "PromotedGuest2",
            Email = "promotedguest2@example.com",
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<SuccessApiResponse<RegisterResponseDto>>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify user data was updated in database
        var (username, email, role) = await GetUserDetailsAsync(guestUserId);
        Assert.Equal("PromotedGuest2", username);
        Assert.Equal("promotedguest2@example.com", email);
        Assert.Equal(0, role); // Should be User role now, not Guest
    }

    [Fact]
    public async Task GuestPromote_SendsConfirmationEmail()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "PromotedGuest3",
            Email = "promotedguest3@example.com",
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<SuccessApiResponse<RegisterResponseDto>>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify confirmation email was sent
        await Task.Delay(500); // Give email time to be sent
        var mailhogClient = Mailhog.CreateClient();
        var emailMessages = await mailhogClient.SearchMessagesByRecipientAsync("promotedguest3@example.com");

        Assert.NotNull(emailMessages);
        Assert.True(emailMessages.Items.Count > 0, "No confirmation email was sent");
    }

    [Fact]
    public async Task GuestPromote_PreservesUserId()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var originalUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "PromotedGuest4",
            Email = "promotedguest4@example.com",
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<SuccessApiResponse<RegisterResponseDto>>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(originalUserId, content!.Data.UserId);

        // Verify in database
        var userExists = await CheckUserExistsAsync(originalUserId);
        Assert.True(userExists);
    }

    [Fact]
    public async Task GuestPromote_SetsPasswordHash()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "PromotedGuest5",
            Email = "promotedguest5@example.com",
            Password = "TestPassword123"
        };

        // Act
        var (response, _, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<SuccessApiResponse<RegisterResponseDto>>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Verify password hash is set in database (not empty)
        var passwordHash = await GetUserPasswordHashAsync(guestUserId);
        Assert.False(string.IsNullOrWhiteSpace(passwordHash));
    }

    /// <summary>
    /// Helper method to get user details from the database
    /// </summary>
    private static async Task<(string Username, string Email, int Role)> GetUserDetailsAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new Npgsql.NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT username, email, role FROM users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return (reader.GetString(0), reader.GetString(1), reader.GetInt32(2));
        }

        throw new InvalidOperationException($"User with ID {userId} not found");
    }

    /// <summary>
    /// Helper method to check if a user exists
    /// </summary>
    private static async Task<bool> CheckUserExistsAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new Npgsql.NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM users WHERE id = @id)";
        cmd.Parameters.AddWithValue("@id", userId);

        var result = await cmd.ExecuteScalarAsync();
        return result != null && (bool)result;
    }

    /// <summary>
    /// Helper method to get user password hash
    /// </summary>
    private static async Task<string> GetUserPasswordHashAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new Npgsql.NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT password_hash FROM users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", userId);

        var result = await cmd.ExecuteScalarAsync();
        return result?.ToString() ?? string.Empty;
    }
}
