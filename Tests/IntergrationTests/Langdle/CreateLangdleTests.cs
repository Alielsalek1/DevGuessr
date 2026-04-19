using System.Net;
using System.Net.Http.Headers;
using Application.DTOs.Auth;
using Application.DTOs.Langdle;
using Application.Utils;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.Langdle;

[Collection("Integration Tests")]
public class CreateLangdleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<HttpClient> GetAuthenticatedClientAsync(string username)
    {
        var email = $"{username.ToLower()}@example.com";
        var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123", Domain.Enums.Roles.Admin);
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);
        
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
        return Client;
    }

    [Fact]
    public async Task Create_WithValidData_Returns201Created()
    {
        var client = await GetAuthenticatedClientAsync("CreateUser");
        var request = new CreateLangdleRequestDto
        {
            Name = "ValidLang",
            YearFirstAppeared = 2025,
            TypeChecking = "STATIC",
            Memory = "MANUAL",
            ScopeSyntax = "BRACES",
            Semicolons = "REQUIRED",
            Tags = ["System", "Fast"]
        };

        var (response, content, json) = await LangdleTestHelpers.CreateAsync<SuccessApiResponse<CreateLangdleResponseDto>>(client, request);

        Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected Created, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal("ValidLang", content.Data.Name);
    }

    [Fact]
    public async Task Create_WithDuplicateName_Returns409Conflict()
    {
        var client = await GetAuthenticatedClientAsync("CreateDupUser");
        var request = new CreateLangdleRequestDto
        {
            Name = "DuplicateLang",
            YearFirstAppeared = 2025,
            TypeChecking = "STATIC",
            Memory = "MANUAL",
            ScopeSyntax = "BRACES",
            Semicolons = "REQUIRED",
            Tags = ["DuplicateTest"]
        };

        var (initialResponse, _, _) = await LangdleTestHelpers.CreateAsync<SuccessApiResponse<CreateLangdleResponseDto>>(client, request);
        Assert.Equal(HttpStatusCode.Created, initialResponse.StatusCode);
        
        var (response, _, _) = await LangdleTestHelpers.CreateAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithInvalidData_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("CreateInvUser");
        var request = new CreateLangdleRequestDto
        {
            Name = "", // Invalid
            YearFirstAppeared = -10, // Invalid
            TypeChecking = "InvalidEnum",
            Memory = "MANUAL",
            ScopeSyntax = "BRACES",
            Semicolons = "REQUIRED",
            Tags = []
        };

        var (response, _, _) = await LangdleTestHelpers.CreateAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
