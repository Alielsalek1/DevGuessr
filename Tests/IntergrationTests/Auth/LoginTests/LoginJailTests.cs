using System.Net;
using Application.DTOs.Auth;
using Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class LoginJailTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private const string RedisPrefix = "Techdle_";

    [Fact]
    public async Task Login_FourFailedAttempts_CreatesJailInRedis()
    {
        // Arrange: Create a verified user
        var (userId, correctPassword, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("JailUser1", "jail1@example.com", "CorrectPassword123");

        var wrongPasswordRequest = new LoginRequestDto
        {
            UsernameOrEmail = username,
            Password = "WrongPassword123"
        };

        // Act: Attempt 4 failed logins with wrong password (jail created on 4th attempt when attempts > 3)
        for (int i = 0; i < 4; i++)
        {
            var (response, _, _) = await LoginTestHelpers.PostLoginAsync<FailApiResponse>(Client, wrongPasswordRequest);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Assert: Verify jail key exists in Redis with 2-hour expiry
        var jailKey = $"jail:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        var jailValue = await Cache.GetStringAsync(jailKey);
        Assert.Equal("true", jailValue);

        // Verify jail key has TTL approximately 2 hours (using raw Redis with prefix)
        var ttl = await Redis.GetTimeToLiveAsync($"{RedisPrefix}{jailKey}");
        Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalHours >= 1.9 && ttl.Value.TotalHours <= 2.0);
    }

    [Fact]
    public async Task Login_FourthFailedAttempt_DoesNotIncrementAttemptsCounter()
    {
        // Arrange: Create a verified user
        var (userId, correctPassword, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("JailUser2", "jail2@example.com", "CorrectPassword123");

        var wrongPasswordRequest = new LoginRequestDto
        {
            UsernameOrEmail = username,
            Password = "WrongPassword123"
        };

        // Act: Attempt 4 failed logins
        for (int i = 0; i < 4; i++)
        {
            await LoginTestHelpers.PostLoginAsync<FailApiResponse>(Client, wrongPasswordRequest);
        }

        // Assert: Verify jail key exists
        var jailKey = $"jail:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        var jailValue = await Cache.GetStringAsync(jailKey);
        Assert.Equal("true", jailValue);
    }

    [Fact]
    public async Task Login_JailedIP_CannotLoginEvenWithCorrectPassword()
    {
        // Arrange: Create a verified user and jail them
        var (userId, correctPassword, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("JailedUser", "jailed@example.com", "CorrectPassword123");

        // Manually create jail entry in Redis using Cache
        var jailKey = $"jail:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        await Cache.SetStringAsync(jailKey, "true", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        });

        var correctPasswordRequest = new LoginRequestDto
        {
            UsernameOrEmail = username,
            Password = correctPassword
        };

        // Act: Try to login with correct password
        var (response, content, _) = await LoginTestHelpers.PostLoginAsync<FailApiResponse>(Client, correctPasswordRequest);

        // Assert: Login should fail with Unauthorized status
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
    }

    [Fact]
    public async Task Login_TwoFailedAttempts_DoesNotCreateJail()
    {
        // Arrange: Create a verified user
        var (userId, correctPassword, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("NoJailUser", "nojail@example.com", "CorrectPassword123");

        var wrongPasswordRequest = new LoginRequestDto
        {
            UsernameOrEmail = username,
            Password = "WrongPassword123"
        };

        // Act: Attempt 2 failed logins (below threshold)
        for (int i = 0; i < 2; i++)
        {
            await LoginTestHelpers.PostLoginAsync<FailApiResponse>(Client, wrongPasswordRequest);
        }

        // Assert: Verify jail key does NOT exist
        var jailKey = $"jail:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        var jailValue = await Cache.GetStringAsync(jailKey);
        Assert.Null(jailValue);

        // Verify attempts counter exists with value "2"
        var attemptsKey = $"login_attempts:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        var attemptsValue = await Cache.GetStringAsync(attemptsKey);
        Assert.Equal("2", attemptsValue);
    }

    [Fact]
    public async Task Login_ThreeFailedAttemptsThenCorrectPassword_SucceedsBeforeJailCreated()
    {
        // Arrange: Create a verified user
        var (userId, correctPassword, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("EdgeCaseUser", "edge@example.com", "CorrectPassword123");

        var wrongPasswordRequest = new LoginRequestDto
        {
            UsernameOrEmail = username,
            Password = "WrongPassword123"
        };

        var correctPasswordRequest = new LoginRequestDto
        {
            UsernameOrEmail = username,
            Password = correctPassword
        };

        // Act: Attempt 3 failed logins, then correct password immediately
        for (int i = 0; i < 3; i++)
        {
            await LoginTestHelpers.PostLoginAsync<FailApiResponse>(Client, wrongPasswordRequest);
        }

        var jailKey = $"jail:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        var jailValueBefore = await Cache.GetStringAsync(jailKey);
        Assert.Null(jailValueBefore);
        
        var (successResponse, successContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, correctPasswordRequest);

        // Assert: Login should succeed
        Assert.Equal(HttpStatusCode.OK, successResponse.StatusCode);
        Assert.True(successContent?.Success);
    }

    [Fact]
    public async Task Login_JailExpiresAfterTwoHours_AllowsLoginAgain()
    {
        // Arrange: Create a verified user and jail them with short expiry
        var (userId, correctPassword, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("ExpiredJailUser", "expiredjail@example.com", "CorrectPassword123");

        var jailKey = $"jail:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        
        // Create jail with 1 second expiry using Cache
        await Cache.SetStringAsync(jailKey, "true", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1)
        });

        // Wait for jail to expire
        await Task.Delay(TimeSpan.FromSeconds(2));

        var correctPasswordRequest = new LoginRequestDto
        {
            UsernameOrEmail = username,
            Password = correctPassword
        };

        // Act: Try to login after jail expires
        var (response, content, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, correctPasswordRequest);

        // Assert: Login should succeed
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(content?.Success);
    }

    [Fact]
    public async Task Login_AttemptsCounterExpiresAfter20Minutes_ResetsCount()
    {
        // Arrange: Create a verified user
        var (userId, correctPassword, username, email) = await AuthBackdoor.CreateVerifiedUserAsync("ResetCountUser", "resetcount@example.com", "CorrectPassword123");

        var wrongPasswordRequest = new LoginRequestDto
        {
            UsernameOrEmail = username,
            Password = "WrongPassword123"
        };

        // Act: Make 2 failed attempts
        for (int i = 0; i < 2; i++)
        {
            await LoginTestHelpers.PostLoginAsync<FailApiResponse>(Client, wrongPasswordRequest);
        }

        // Manually expire the attempts counter using Cache.Remove (simulates expiry)
        var attemptsKey = $"login_attempts:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        await Cache.RemoveAsync(attemptsKey);

        // Make 2 more failed attempts
        for (int i = 0; i < 2; i++)
        {
            await LoginTestHelpers.PostLoginAsync<FailApiResponse>(Client, wrongPasswordRequest);
        }

        // Assert: Attempts counter should be at 2
        var attemptsValue = await Cache.GetStringAsync(attemptsKey);
        Assert.Equal("2", attemptsValue);

        var jailKey = $"jail:{userId}:{FakeRemoteIpAddressMiddleware.DefaultTestIpAddress}";
        var jailValue = await Cache.GetStringAsync(jailKey);
        Assert.Null(jailValue);
    }
}
