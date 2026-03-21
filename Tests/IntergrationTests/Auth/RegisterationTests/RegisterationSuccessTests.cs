using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Tests.MailHog;
using Application.DTOs.Auth;
using Application.Utils;
using Tests.Common;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class RegisterationSuccessTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Register_WithAddresss_Returns201CreatedWithUserId()
    {
        var request = new RegisterRequestDto
        {
            Username = "ValidUser123",
            Email = "user@example.com",
            Password = "TestPassword123",
            Address = "123 Main Street"
        };

        var (response, content, _) = await RegisterationTestHelpers.PostRegisterAsync<SuccessApiResponse<RegisterResponseDto>>(Client, request);

        AssertRegistrationSuccess(response, content);
    }

    [Fact]
    public async Task Register_WithPhoneNumber_Returns201CreatedWithUserId()
    {
        var request = new RegisterRequestDto
        {
            Username = "ValidUser123",
            Email = "user@example.com",
            Password = "TestPassword123",
            PhoneNumber = "+1234567890",
        };

        var (response, content, _) = await RegisterationTestHelpers.PostRegisterAsync<SuccessApiResponse<RegisterResponseDto>>(Client, request);

        AssertRegistrationSuccess(response, content);
    }

    [Fact]
    public async Task Register_WithMinimalValidData_Returns201CreatedWithUserIdAsync()
    {
        var request = new RegisterRequestDto
        {
            Username = "MinimalUser",
            Email = "minimal@example.com",
            Password = "MinimalPass123"
        };

        var (response, content, _) = await RegisterationTestHelpers.PostRegisterAsync<SuccessApiResponse<RegisterResponseDto>>(Client, request);

        AssertRegistrationSuccess(response, content);
    }

    [Fact]
    public async Task Register_PutsTokenInRedisCacheWithCorrectExpiration()
    {
        var Email = "redis@example.com";
        var request = new RegisterRequestDto
        {
            Username = "RedisTestUser",
            Email = Email,
            Password = "TestPassword123"
        };

        var (response, content, _) = await RegisterationTestHelpers.PostRegisterAsync<SuccessApiResponse<RegisterResponseDto>>(Client, request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Fetch the token from Mailhog
        MailhogMessage? message = null;
        for (int i = 0; i < 5; i++)
        {
            var messages = await Mailhog.GetAllMessagesAsync();
            message = messages.Items.FirstOrDefault(m => m.To.Any(t => t.Email == Email));
            if (message != null) break;
            await Task.Delay(500);
        }
        
        Assert.NotNull(message);
        
        var token = RegisterationTestHelpers.ExtractTokenFromBody(message.Content.Body);
        Assert.False(string.IsNullOrEmpty(token), "Token should not be null or empty");

        // Use the native Cache (IDistributedCache) to verify the value
        // It handles the "Techdle_" prefix automatically
        var storedUserId = await Cache.GetStringAsync($"new_user:{token}");
        Assert.NotNull(storedUserId);
        
        // Use Redis provider only for TTL or low-level checks if needed, but with correct key
        var redisKey = $"Techdle_new_user:{token}";
        var ttl = await Redis.GetTTLAsync(redisKey);
        Assert.True(ttl > 0);
        Assert.True(ttl <= 600); // 10 minutes
    }

    [Fact]
    public async Task Register_IsIdempotent_ReturnsSameSuccessResponseForSameKey()
    {
        var request = new RegisterRequestDto
        {
            Username = "IdempotentUser",
            Email = "idempotent@example.com",
            Password = "IdempotentPass123"
        };
        var idempotencyKey = Guid.NewGuid().ToString();

        // First request
        var (response1, content1, json1) = await RegisterationTestHelpers.PostRegisterAsync<SuccessApiResponse<RegisterResponseDto>>(Client, request, idempotencyKey);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Second request with same key
        var (response2, content2, json2) = await RegisterationTestHelpers.PostRegisterAsync<SuccessApiResponse<RegisterResponseDto>>(Client, request, idempotencyKey);

        // Assertions
        Assert.Equal(HttpStatusCode.Created, response2.StatusCode);
        Assert.Equal(json1, json2);
    }

    private static void AssertRegistrationSuccess(HttpResponseMessage response, SuccessApiResponse<RegisterResponseDto>? content)
    {
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal(201, content.StatusCode);
        Assert.Equal("User registered successfully. Please check your email for the confirmation code.", content.Message);
        Assert.NotEqual(Guid.Empty, content.Data.UserId);
    }
}
