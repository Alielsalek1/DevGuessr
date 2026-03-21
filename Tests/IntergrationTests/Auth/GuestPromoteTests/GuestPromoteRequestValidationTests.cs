using System.Net;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Application.Utils;
using Tests.Common;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class GuestPromoteRequestValidationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GuestPromote_WithoutAuthentication_Returns401Unauthorized()
    {
        // Arrange
        var promoteRequest = new RegisterRequestDto
        {
            Username = "TestUser",
            Email = "test@example.com",
            Password = "TestPassword123"
        };

        // Act: Try to promote without providing an access token
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal-auth/promote/guest")
        {
            Content = JsonContent.Create(promoteRequest)
        };

        var response = await Client.SendAsync(requestMessage);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GuestPromote_WithInvalidAccessToken_Returns401Unauthorized()
    {
        // Arrange
        var promoteRequest = new RegisterRequestDto
        {
            Username = "TestUser",
            Email = "test@example.com",
            Password = "TestPassword123"
        };

        // Act: Use an invalid access token
        var (response, _, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, "invalid.token.here");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GuestPromote_WithEmptyUsername_Returns400BadRequest()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "",
            Email = "test@example.com",
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("username", content.Message.ToLower());
    }

    [Fact]
    public async Task GuestPromote_WithEmptyEmail_Returns400BadRequest()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "TestUser",
            Email = "",
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("email", content.Message.ToLower());
    }

    [Fact]
    public async Task GuestPromote_WithInvalidEmail_Returns400BadRequest()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "TestUser",
            Email = "not-a-valid-email",
            Password = "TestPassword123"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("email", content.Message.ToLower());
    }

    [Fact]
    public async Task GuestPromote_WithEmptyPassword_Returns400BadRequest()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "TestUser",
            Email = "test@example.com",
            Password = ""
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("password", content.Message.ToLower());
    }

    // Removed: Password confirmation is not part of RegisterRequestDto

    [Fact]
    public async Task GuestPromote_WithShortPassword_Returns400BadRequest()
    {
        // Arrange: Create a guest user
        var (loginResponse, loginContent, _, _) = await GuestLoginTestHelpers.PostGuestLoginAsync<SuccessApiResponse<GuestLoginResponseDto>>(Client);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var accessToken = loginContent!.Data.AccessToken;

        var promoteRequest = new RegisterRequestDto
        {
            Username = "TestUser",
            Email = "test@example.com",
            Password = "abc"
        };

        // Act
        var (response, content, _) = await GuestPromoteTestHelpers.PostGuestPromoteAsync<FailApiResponse>(Client, promoteRequest, accessToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("password", content.Message.ToLower());
    }
}