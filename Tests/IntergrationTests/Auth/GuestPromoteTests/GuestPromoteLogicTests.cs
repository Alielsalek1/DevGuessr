using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.DTOs.Auth;
using Application.Utils;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class GuestPromoteLogicTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GuestPromote_WithDuplicateEmail_Returns409Conflict()
    {
        // Arrange: Create a verified user with an email
        var (_, _, _, existingEmail) = await AuthBackdoor.CreateVerifiedUserAsync("ExistingUser", "existing@example.com", "TestPassword123");

        // Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "NewUsername",
            Email = existingEmail, // Use the existing email
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("email", content.Message.ToLower());
    }

    [Fact]
    public async Task GuestPromote_WithDuplicateUsername_Returns409Conflict()
    {
        // Arrange: Create a verified user with a username
        var (_, _, existingUsername, _) = await AuthBackdoor.CreateVerifiedUserAsync("ExistingUsername", "existing2@example.com", "TestPassword123");

        // Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = existingUsername, // Use the existing username
            Email = "newunique@example.com",
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("username", content.Message.ToLower());
    }

    [Fact]
    public async Task GuestPromote_WithNonGuestUser_Returns400BadRequest()
    {
        // Arrange: Create a regular verified user (not a guest)
        var (userId, _, _, _) = await AuthBackdoor.CreateVerifiedUserAsync("RegularUser", "regular@example.com", "TestPassword123");

        // Get an access token for this user (simulate login)
        var loginRequest = new LoginRequestDto
        {
            UsernameOrEmail = "RegularUser",
            Password = "TestPassword123"
        };

        var loginRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal-auth/login")
        {
            Content = JsonContent.Create(loginRequest)
        };

        var loginResponse = await Client.SendAsync(loginRequestMessage);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginJson = await loginResponse.Content.ReadAsStringAsync();
        var loginContent = JsonSerializer.Deserialize<SuccessApiResponse<LoginResponseDto>>(loginJson, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "NewUsername2",
            Email = "newemail2@example.com",
            Password = "TestPassword123"
        };

        // Act: Try to promote a non-guest user
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("guest", content.Message.ToLower());
    }

    [Fact]
    public async Task GuestPromote_WithNonExistentUser_Returns404NotFound()
    {
        // Arrange: Create a fake JWT token with a non-existent user ID
        // For this test, we'll use a valid guest user token but manually construct one with wrong ID
        // Since we can't easily forge a valid JWT, we'll create a guest, get their token, 
        // then delete them from DB to simulate a non-existent user

        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var guestUserId = loginContent!.Data.UserId;
        var accessToken = loginContent.Data.AccessToken;

        // Delete the user from the database
        await DeleteUserAsync(guestUserId);

        var promoteRequest = new RegisterRequestDto
        {
            Username = "NewUsername3",
            Email = "newemail3@example.com",
            Password = "TestPassword123"
        };

        // Act: Try to promote a non-existent user
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
    }

    [Fact]
    public async Task GuestPromote_CannotPromoteSameGuestTwice()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var firstPromoteRequest = new RegisterRequestDto
        {
            Username = "FirstPromotion",
            Email = "firstpromotion@example.com",
            Password = "TestPassword123"
        };

        // Act: First promotion
        var (firstResponse, _, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<SuccessApiResponse<RegisterResponseDto>>(Client, firstPromoteRequest, accessToken);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Try to promote again with different credentials
        var secondPromoteRequest = new RegisterRequestDto
        {
            Username = "SecondPromotion",
            Email = "secondpromotion@example.com",
            Password = "TestPassword123"
        };

        var (secondResponse, secondContent, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, secondPromoteRequest, accessToken);

        // Assert: Second promotion should fail
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
        Assert.NotNull(secondContent);
        Assert.False(secondContent.Success);
        Assert.Contains("guest", secondContent.Message.ToLower());
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
}
