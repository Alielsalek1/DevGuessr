using System.Net;
using Application.DTOs.Auth;
using Application.Utils;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class ResetPasswordLogicTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ResetPassword_WithInvalidToken_Returns400BadRequest()
    {
        // Arrange
        var (userId, _, _, email) = await AuthBackdoor.CreateVerifiedUserAsync("InvalidTokenReset", "invalidtoken@example.com", "TestPassword123");

        // Set token in Redis
        await AuthBackdoor.SeedPasswordResetTokenAsync(Factory, userId, "111111");

        var request = new ResetPasswordRequestDto
        {
            Otp = "wrong-token",
            NewPassword = "NewPassword123"
        };

        // Act
        var (response, content, _) = await ResetPasswordTestHelpers.PostResetPasswordAsync<FailApiResponse>(Client, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
    }

    [Fact]
    public async Task ResetPassword_WithNonExistentUser_Returns400BadRequest()
    {
        // Arrange
        var request = new ResetPasswordRequestDto
        {
            Otp = "123456",
            NewPassword = "NewPassword123"
        };

        // Act
        var (response, content, _) = await ResetPasswordTestHelpers.PostResetPasswordAsync<FailApiResponse>(Client, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithUnverifiedUser_Returns400BadRequest()
    {
        // Arrange
        var email = "unverified-reset@example.com";
        await AuthBackdoor.CreateUnverifiedUserAsync("UnverifiedReset", email, "TestPassword123");

        var request = new ResetPasswordRequestDto
        {
            Otp = "123456",
            NewPassword = "NewPassword123"
        };

        // Act
        var (response, content, _) = await ResetPasswordTestHelpers.PostResetPasswordAsync<FailApiResponse>(Client, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
