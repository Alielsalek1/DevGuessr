using System.Net;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Application.Utils;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class ResetPasswordSuccessTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ResetPassword_WithValidToken_Returns200Ok_AndUpdatesPassword()
    {
        // Arrange
        var (userId, oldPassword, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("ResetUser", "reset@example.com", "OldPassword123");

        // Generate token and store in Redis
        var token = "123456";
        await AuthBackdoor.SeedPasswordResetTokenAsync(Factory, userId, token);

        var newPassword = "NewPassword123";
        var request = new ResetPasswordRequestDto
        {
            Otp = token,
            NewPassword = newPassword
        };

        // Act
        var (response, content, _) = await ResetPasswordTestHelpers.PostResetPasswordAsync<SuccessApiResponse>(Client, request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);

        // Verify login with new password works
        var deviceId = Guid.NewGuid();
        await AuthBackdoor.SeedUserDeviceAsync(userId, deviceId);

        var loginRequest = new LoginRequestDto
        {
            UsernameOrEmail = email,
            Password = newPassword
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/internal-auth/login")
        {
            Content = JsonContent.Create(loginRequest)
        };
        requestMessage.Headers.Add("Cookie", $"deviceId={deviceId}");

        var loginResponse = await Client.SendAsync(requestMessage);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }
}
