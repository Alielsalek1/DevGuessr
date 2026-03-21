using System.Net;
using Application.DTOs.Auth;
using Application.DTOs.ExternalAuth;
using Application.Utils;
using Tests.Common;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class LinkGoogleSuccessTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task LinkGoogle_WithGuestUser_LinksAccountAndReturnsTokens()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        // Configure test validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "linkeduser@example.com", "Linked User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, content, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal("Google authentication successful.", content.Message);
        Assert.NotNull(content.Data.AccessToken);
        Assert.Equal(guestUserId, content.Data.UserId);
    }

    [Fact]
    public async Task LinkGoogle_UpdatesGuestUserToRegularUser()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        // Configure test validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "updateduser@example.com", "Updated User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, content, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify user was updated in database
        var (username, email, role) = await GetUserDetailsAsync(guestUserId);
        Assert.Equal("updateduser@example.com", email);
        Assert.Equal(0, role); // Should be User role now, not Guest
    }

    [Fact]
    public async Task LinkGoogle_PreservesUserId()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var originalUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        // Configure test validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "preserveduser@example.com", "Preserved User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, content, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(originalUserId, content!.Data.UserId);

        // Verify in database
        var userExists = await CheckUserExistsAsync(originalUserId);
        Assert.True(userExists);
    }

    [Fact]
    public async Task LinkGoogle_SetsExternalAuthScheme()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        // Configure test validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "externaluser@example.com", "External User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, _, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify auth scheme was set to External in database
        var authScheme = await GetUserAuthSchemeAsync(guestUserId);
        Assert.Equal(1, authScheme); // External = 1
    }

    [Fact]
    public async Task LinkGoogle_MarksEmailAsVerified()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        // Configure test validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "verifieduser@example.com", "Verified User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, _, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify email is marked as verified
        var isVerified = await IsEmailVerifiedAsync(guestUserId);
        Assert.True(isVerified);
    }

    [Fact]
    public async Task LinkGoogle_GeneratesNewAccessToken()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var originalAccessToken = loginContent!.Data.AccessToken;
        var accessToken = loginContent.Data.AccessToken;

        // Configure test validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "newtokenuser@example.com", "New Token User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, content, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content!.Data.AccessToken);
        Assert.NotEqual(originalAccessToken, content.Data.AccessToken); // Should be a new token
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
            var username = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            var email = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var role = reader.GetInt32(2);
            return (username, email, role);
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
    /// Helper method to check if user has Google authentication (external auth)
    /// Returns 1 (External) if google_id is set, 0 (Internal) otherwise
    /// </summary>
    private static async Task<int> GetUserAuthSchemeAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new Npgsql.NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT google_id FROM users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", userId);

        var result = await cmd.ExecuteScalarAsync();
        // If google_id is set (not null), user has External auth (1), otherwise Internal (0)
        return result != null && result != DBNull.Value ? 1 : 0;
    }

    /// <summary>
    /// Helper method to check if email is verified
    /// </summary>
    private static async Task<bool> IsEmailVerifiedAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new Npgsql.NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT is_email_verified FROM users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", userId);

        var result = await cmd.ExecuteScalarAsync();
        return result != null && (bool)result;
    }
}
