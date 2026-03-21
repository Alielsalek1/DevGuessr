using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Application.Utils;
using Tests.Common;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class GuestLoginSuccessTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GuestLogin_Returns200OkWithTokensAndUserId()
    {
        // Act
        var (response, content, _, refreshTokenCookie) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal(200, content.StatusCode);
        Assert.Equal("Guest login successful.", content.Message);

        // Verify response data
        Assert.NotNull(content.Data);
        Assert.NotEqual(Guid.Empty, content.Data.UserId);
        Assert.False(string.IsNullOrWhiteSpace(content.Data.AccessToken));

        // Verify refresh token cookie is set (RefreshToken is JsonIgnored in response)
        Assert.NotNull(refreshTokenCookie);
    }

    [Fact]
    public async Task GuestLogin_CreatesUserInDatabase()
    {
        // Act
        var (response, content, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);

        // Verify user exists in database
        var userId = content!.Data.UserId;
        var userExists = await CheckUserExistsInDatabaseAsync(userId);
        Assert.True(userExists);
    }

    [Fact]
    public async Task GuestLogin_CreatesUserWithGuestRole()
    {
        // Act
        var (response, content, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);

        // Verify user has Guest role (role = 2)
        var userId = content!.Data.UserId;
        var userRole = await GetUserRoleAsync(userId);
        Assert.Equal(2, userRole); // Guest role enum value
    }

    [Fact]
    public async Task GuestLogin_AccessTokenCanBeUsedForAuthorization()
    {
        // Act: Login as guest
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var accessToken = loginContent!.Data!.AccessToken;

        // Act: Use the access token to access a protected endpoint
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var authResponse = await Client.GetAsync("/health/auth");

        // Assert: Should be able to access protected endpoint
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);
        var authContent = await authResponse.Content.ReadAsStringAsync();
        Assert.Contains("Authenticated", authContent);

        Client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GuestLogin_RefreshTokenCanBeUsedToRefreshAccessToken()
    {
        // Act: Login as guest
        var (loginResponse, loginContent, _, refreshTokenCookie) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(refreshTokenCookie);

        var userId = loginContent!.Data.UserId;

        // Act: Use the refresh token to get a new access token
        var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal-auth/refresh-token")
        {
            Content = JsonContent.Create(new { userId })
        };
        refreshRequest.Headers.Add("Cookie", $"refreshToken={refreshTokenCookie}");

        var refreshResponse = await Client.SendAsync(refreshRequest);

        // Assert: Should successfully refresh the token
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
    }

    [Fact]
    public async Task GuestLogin_MultipleCallsCreateDifferentUsers()
    {
        // Act: Login as guest twice
        var (response1, content1, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        var (response2, content2, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);

        // Assert: Both requests succeed
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

        // Assert: Different user IDs are created
        Assert.NotEqual(content1!.Data.UserId, content2!.Data.UserId);
        Assert.NotEqual(content1.Data.AccessToken, content2.Data.AccessToken);
    }

    /// <summary>
    /// Helper method to check if a user exists in the database
    /// </summary>
    private static async Task<bool> CheckUserExistsInDatabaseAsync(Guid userId)
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
    /// Helper method to get the role of a user from the database
    /// </summary>
    private static async Task<int> GetUserRoleAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new Npgsql.NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT role FROM users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", userId);

        var result = await cmd.ExecuteScalarAsync();
        return result != null ? (int)result : -1;
    }
}
