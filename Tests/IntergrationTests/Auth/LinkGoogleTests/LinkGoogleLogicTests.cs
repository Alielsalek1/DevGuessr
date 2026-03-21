using System.Net;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Application.DTOs.ExternalAuth;
using Application.Utils;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class LinkGoogleLogicTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task LinkGoogle_WithNonGuestUser_ReturnsBadRequest()
    {
        // Arrange: Create a verified regular user (not guest)
        var (userId, password, username, email) = await AuthBackdoor.CreateVerifiedUserAsync();

        // Login to get access token
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var loginResponse = await Client.PostAsJsonAsync("/api/v1/internal-auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginContent = await loginResponse.Content.ReadFromJsonAsync<SuccessApiResponse<LoginResponseDto>>();
        var accessToken = loginContent!.Data.AccessToken;

        // Setup validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "newgoogleuser@example.com", "New Google User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, content, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<FailApiResponse>(Client, request, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
    }

    [Fact]
    public async Task LinkGoogle_WithEmailAlreadyInUse_ReturnsBadRequest()
    {
        // Arrange: Create a regular user with a specific email
        var existingEmail = "existing@example.com";
        var (userId, password, username, email) = await AuthBackdoor.CreateVerifiedUserAsync(email: existingEmail);

        // Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestAccessToken = loginContent!.Data.AccessToken;

        // Setup validator to return the existing email
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, existingEmail, "Existing User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, content, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<FailApiResponse>(Client, request, guestAccessToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
    }

    [Fact]
    public async Task LinkGoogle_WithAdminUser_ReturnsBadRequest()
    {
        // Arrange: Create an admin user
        var email = "admin@example.com";
        var (adminUserId, password, username, _) = await AuthBackdoor.CreateVerifiedUserAsync(email: email);

        // Manually set user to admin role
        await SetUserRoleAsync(adminUserId, 1); // Admin role

        // Login to get access token
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var loginResponse = await Client.PostAsJsonAsync("/api/v1/internal-auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginContent = await loginResponse.Content.ReadFromJsonAsync<SuccessApiResponse<LoginResponseDto>>();
        var accessToken = loginContent!.Data.AccessToken;

        // Setup validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "admingoogleuser@example.com", "Admin Google User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, content, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<FailApiResponse>(Client, request, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
    }

    [Fact]
    public async Task LinkGoogle_TwiceWithSameGuestUser_SecondAttemptFails()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        // Setup validator for first link
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "firstlink@example.com", "First Link");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act: First link - should succeed
        var (firstResponse, firstContent, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request, accessToken);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        // Get new access token from first successful link
        var newAccessToken = firstContent!.Data.AccessToken;

        // Setup validator for second attempt
        const string idToken2 = "valid-token-2";
        TestGoogleAuthValidator.ConfigureValidToken(idToken2, "secondlink@example.com", "Second Link");

        var request2 = new GoogleAuthRequestDto { IdToken = idToken2 };

        // Act: Second link - should fail (user is no longer guest)
        var (secondResponse, secondContent, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<FailApiResponse>(Client, request2, newAccessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
        Assert.NotNull(secondContent);
        Assert.False(secondContent.Success);
    }

    [Fact]
    public async Task LinkGoogle_WithDeletedUser_ReturnsUnauthorized()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        // Delete the user
        await DeleteUserAsync(guestUserId);

        // Setup validator
        const string idToken = "valid-token";
        TestGoogleAuthValidator.ConfigureValidToken(idToken, "deleteduser@example.com", "Deleted User");

        var request = new GoogleAuthRequestDto { IdToken = idToken };

        // Act
        var (response, _, json) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<FailApiResponse>(Client, request, accessToken);

        // Assert
        // Should return NotFound because the user was deleted
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task LinkGoogle_WithDifferentGuestUsers_BothSucceed()
    {
        // Arrange: Create two guest users
        var (login1Response, login1Content, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, login1Response.StatusCode);
        var guest1AccessToken = login1Content!.Data.AccessToken;

        var (login2Response, login2Content, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, login2Response.StatusCode);
        var guest2AccessToken = login2Content!.Data.AccessToken;

        // Setup validators for both
        const string idToken1 = "valid-token-1";
        TestGoogleAuthValidator.ConfigureValidToken(idToken1, "guest1link@example.com", "Guest 1 Link");

        const string idToken2 = "valid-token-2";
        TestGoogleAuthValidator.ConfigureValidToken(idToken2, "guest2link@example.com", "Guest 2 Link");

        var request1 = new GoogleAuthRequestDto { IdToken = idToken1 };
        var request2 = new GoogleAuthRequestDto { IdToken = idToken2 };

        // Act
        var (response1, content1, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request1, guest1AccessToken);
        var (response2, content2, _) = await LinkGoogleTestHelpers.PostLinkGoogleAsync<SuccessApiResponse<GoogleAuthResponseDto>>(Client, request2, guest2AccessToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.NotNull(content1);
        Assert.True(content1.Success);

        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.NotNull(content2);
        Assert.True(content2.Success);

        // Verify both users were updated
        Assert.NotEqual(content1.Data.UserId, content2.Data.UserId);
    }

    /// <summary>
    /// Helper method to delete a user from the database
    /// </summary>
    private static async Task DeleteUserAsync(Guid userId)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new Npgsql.NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", userId);

        await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Helper method to set user role in the database
    /// </summary>
    private static async Task SetUserRoleAsync(Guid userId, int role)
    {
        var connStr = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (string.IsNullOrEmpty(connStr)) throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

        await using var conn = new Npgsql.NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE users SET role = @role WHERE id = @id";
        cmd.Parameters.AddWithValue("@role", role);
        cmd.Parameters.AddWithValue("@id", userId);

        await cmd.ExecuteNonQueryAsync();
    }
}
