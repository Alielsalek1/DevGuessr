using System.Net;
using System.Net.Http.Headers;
using Application.DTOs.Auth;
using Application.DTOs.TechnectionCategory;
using Application.Utils;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.TechnectionCategories;

[Collection("Integration Tests")]
public class CreateTechnectionCategoryTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
        var client = await GetAuthenticatedClientAsync("TechCrtUsr1");
        var request = TechnectionCategoriesTestHelpers.BuildValidRequest("ValidGroup1");

        var (response, content, json) = await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateTechnectionCategoryResponseDto>>(client, request);

        Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected Created, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal("ValidGroup1", content.Data.GroupName);
        Assert.Equal(1, content.Data.DifficultyLevel);
        Assert.Equal(4, content.Data.Words.Count);
    }

    [Fact]
    public async Task Create_WithDuplicateGroupName_Returns409Conflict()
    {
        var client = await GetAuthenticatedClientAsync("TechCrtDup1");
        var request = TechnectionCategoriesTestHelpers.BuildValidRequest("DuplicateGroup");

        var (initialResponse, _, _) = await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateTechnectionCategoryResponseDto>>(client, request);
        Assert.Equal(HttpStatusCode.Created, initialResponse.StatusCode);

        var (response, _, _) = await TechnectionCategoriesTestHelpers.CreateAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithInvalidDifficultyLevel_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("TechCrtInv1");
        var request = TechnectionCategoriesTestHelpers.BuildValidRequest("InvalidDiffGroup", difficultyLevel: 5);

        var (response, _, _) = await TechnectionCategoriesTestHelpers.CreateAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithEmptyGroupName_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("TechCrtInv2");
        var request = TechnectionCategoriesTestHelpers.BuildValidRequest(groupName: "");

        var (response, _, _) = await TechnectionCategoriesTestHelpers.CreateAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithEmptyWords_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("TechCrtInv3");
        var request = TechnectionCategoriesTestHelpers.BuildValidRequest("EmptyWordsGroup", words: []);

        var (response, _, _) = await TechnectionCategoriesTestHelpers.CreateAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
