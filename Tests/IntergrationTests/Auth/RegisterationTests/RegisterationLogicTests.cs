using System.Net;
using Application.DTOs.Auth;
using Application.Utils;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common;
using Xunit;

namespace Tests.Auth;

[Collection("Integration Tests")]
public class RegisterationLogicTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Register_WithDuplicateUsername_Returns400BadRequest()
    {
        var firstRequest = new RegisterRequestDto
        {
            Username = "DuplicateUser",
            Email = "first@example.com",
            Password = "Password123"
        };

        await RegisterationTestHelpers.PostRegisterAsync<object>(Client, firstRequest);

        var secondRequest = new RegisterRequestDto
        {
            Username = "DuplicateUser",
            Email = "second@example.com",
            Password = "Password123"
        };

        var (response, content, _) = await RegisterationTestHelpers.PostRegisterAsync<FailApiResponse>(Client, secondRequest);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("username", content.Message.ToLower());
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400BadRequest()
    {
        var firstRequest = new RegisterRequestDto
        {
            Username = "FirstUser",
            Email = "duplicate@example.com",
            Password = "Password123"
        };

        await RegisterationTestHelpers.PostRegisterAsync<object>(Client, firstRequest);

        var secondRequest = new RegisterRequestDto
        {
            Username = "SecondUser",
            Email = "duplicate@example.com",
            Password = "Password123"
        };

        var (response, content, _) = await RegisterationTestHelpers.PostRegisterAsync<FailApiResponse>(Client, secondRequest);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Contains("email", content.Message.ToLower());
    }

    [Fact]
    public async Task Register_StoresPasswordHashWithCorrectLength()
    {
        var request = new RegisterRequestDto
        {
            Username = "PasswordTest",
            Email = "pwd@example.com",
            Password = "PlainTextPassword123"
        };

        var (_, content, _) = await RegisterationTestHelpers.PostRegisterAsync<SuccessApiResponse<RegisterResponseDto>>(Client, request);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.FindAsync(content!.Data.UserId);

        Assert.NotNull(user!.PasswordHash);
        // BCrypt generates 60-character hashes
        Assert.Equal(60, user.PasswordHash.Length);
    }

    [Fact]
    public async Task Register_StoresRefreshTokenLifeTimeCorrectly()
    {
        var Email = "refresh@example.com";
        var request = new RegisterRequestDto
        {
            Username = "RefreshTokenTest",
            Email = Email,
            Password = "Password123"
        };

        var (_, content, _) = await RegisterationTestHelpers.PostRegisterAsync<SuccessApiResponse<RegisterResponseDto>>(Client, request);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == Email);

        Assert.NotNull(user);

        // Verify that no refresh token is created during registration (only after first login)
        var refreshTokens = await dbContext.UserRefreshTokens.Where(rt => rt.UserId == user.Id).ToListAsync();
        Assert.Empty(refreshTokens); // Registration doesn't create refresh tokens anymore
    }
}