using System.Net;
using System.Net.Http.Headers;
using Application.DTOs.Auth;
using Application.DTOs.Clusterdle;
using Application.Utils;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.TechnectionCategories;

[Collection("Integration Tests")]
public class UpdateClusterdleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task Update_WithValidData_Returns200OK()
    {
        var client = await GetAuthenticatedClientAsync("TechUpdUsr1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("UpdGroup1"));

        var updateRequest = new UpdateClusterdleByGroupNameRequestDto
        {
            DifficultyLevel = 3,
            SuccessMessage = "Well done on group!"
        };

        var (response, content, json) = await TechnectionCategoriesTestHelpers.UpdateAsync<SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>>(
            client, "UpdGroup1", updateRequest);

        Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.Equal(3, content.Data.DifficultyLevel);
        Assert.Equal("Well done on group!", content.Data.SuccessMessage);
        // Words must be untouched
        Assert.Equal(4, content.Data.Words.Count);
    }

    [Fact]
    public async Task Update_DoesNotModifyWords()
    {
        var client = await GetAuthenticatedClientAsync("TechUpdUsr2");
        var originalWords = new List<string> { "alpha", "beta", "gamma", "delta" };
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("UpdWordGroup", words: originalWords));

        var updateRequest = new UpdateClusterdleByGroupNameRequestDto { SuccessMessage = "Changed message" };

        var (_, content, _) = await TechnectionCategoriesTestHelpers.UpdateAsync<SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>>(
            client, "UpdWordGroup", updateRequest);

        Assert.NotNull(content);
        Assert.Equal(originalWords.Count, content.Data.Words.Count);
        foreach (var word in originalWords)
        {
            Assert.Contains(word, content.Data.Words);
        }
    }

    [Fact]
    public async Task Update_WithDuplicateGroupName_Returns409Conflict()
    {
        var client = await GetAuthenticatedClientAsync("TechUpdDup1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("UpdConflict1"));
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("UpdConflict2"));

        var updateRequest = new UpdateClusterdleByGroupNameRequestDto { GroupName = "UpdConflict2" };

        var (response, _, _) = await TechnectionCategoriesTestHelpers.UpdateAsync<FailApiResponse>(
            client, "UpdConflict1", updateRequest);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithNonExistentCategory_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("TechUpdMiss1");
        var updateRequest = new UpdateClusterdleByGroupNameRequestDto { SuccessMessage = "Nope" };

        var (response, _, _) = await TechnectionCategoriesTestHelpers.UpdateAsync<FailApiResponse>(
            client, "NonExistentGroup", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithInvalidDifficultyLevel_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("TechUpdInv1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("UpdInvGroup"));

        var updateRequest = new UpdateClusterdleByGroupNameRequestDto { DifficultyLevel = 99 };

        var (response, _, _) = await TechnectionCategoriesTestHelpers.UpdateAsync<FailApiResponse>(
            client, "UpdInvGroup", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
