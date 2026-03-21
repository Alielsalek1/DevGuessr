using System.Net;
using Application.DTOs.ExternalAuth;
using Application.Services.Interfaces;
using Application.Utils;
using Google.Apis.Auth;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Auth.ExternalLoginTests;

[Collection("Integration Tests")]
public class GoogleLoginTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GoogleLogin_WithNewUser_CreatesUserAndReturnsTokens()
    {
        // Arrange
        var mockValidator = new Mock<IGoogleAuthValidator>();
        var payload = new GoogleJsonWebSignature.Payload
        {
            Email = "newuser@example.com",
            Name = "New User",
            Subject = "google_12345678901234567890" // Google ID (required for user creation)
        };
        mockValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
            .ReturnsAsync(payload);

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => mockValidator.Object);
            });
        }).CreateClient();

        var request = new GoogleAuthRequestDto { IdToken = "valid-token" };

        // Act
        var (response, content, _) = await ExternalLoginTestHelpers.PostGoogleLoginAsync<SuccessApiResponse<GoogleAuthResponseDto>>(client, request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal("Google authentication successful.", content.Message);
        Assert.NotNull(content.Data.AccessToken);
        Assert.NotEqual(Guid.Empty, content.Data.UserId);
    }

    [Fact]
    public async Task GoogleLogin_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var mockValidator = new Mock<IGoogleAuthValidator>();
        mockValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<GoogleJsonWebSignature.ValidationSettings>()))
            .ThrowsAsync(new InvalidJwtException("Invalid token"));

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => mockValidator.Object);
            });
        }).CreateClient();

        var request = new GoogleAuthRequestDto { IdToken = "invalid-token" };

        // Act
        var (response, _, _) = await ExternalLoginTestHelpers.PostGoogleLoginAsync<FailApiResponse>(client, request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
