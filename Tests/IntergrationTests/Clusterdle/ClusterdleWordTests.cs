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
public class ClusterdleWordTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task AddWord_WithValidWord_Returns200OKAndWordIsAdded()
    {
        var client = await GetAuthenticatedClientAsync("TechWrdAdd1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("WordGrp1"));

        var addRequest = new AddClusterdleWordByGroupNameRequestDto { Word = "rebase" };
        var (response, content, json) = await TechnectionCategoriesTestHelpers.AddWordAsync<SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>>(
            client, "WordGrp1", addRequest);

        Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.Contains("rebase", content.Data.Words);
        Assert.Equal(5, content.Data.Words.Count); // original 4 + 1
    }

    [Fact]
    public async Task AddWord_DuplicateWord_Returns409Conflict()
    {
        var client = await GetAuthenticatedClientAsync("TechWrdDup1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("WordGrpDup", words: ["commit", "push", "pull", "merge"]));

        var addRequest = new AddClusterdleWordByGroupNameRequestDto { Word = "commit" };
        var (response, _, _) = await TechnectionCategoriesTestHelpers.AddWordAsync<FailApiResponse>(
            client, "WordGrpDup", addRequest);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddWord_ToNonExistentCategory_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("TechWrdMiss1");

        var addRequest = new AddClusterdleWordByGroupNameRequestDto { Word = "stash" };
        var (response, _, _) = await TechnectionCategoriesTestHelpers.AddWordAsync<FailApiResponse>(
            client, "NonExistentGroup", addRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveWord_WithExistingWord_Returns200OKAndWordIsRemoved()
    {
        var client = await GetAuthenticatedClientAsync("TechWrdRmv1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("WordGrpRmv", words: ["commit", "push", "pull", "merge"]));

        var removeRequest = new RemoveClusterdleWordByGroupNameRequestDto { Word = "commit" };
        var (response, content, json) = await TechnectionCategoriesTestHelpers.RemoveWordAsync<SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>>(
            client, "WordGrpRmv", removeRequest);

        Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.DoesNotContain("commit", content.Data.Words);
        Assert.Equal(3, content.Data.Words.Count);
    }

    [Fact]
    public async Task RemoveWord_WithNonExistentWord_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("TechWrdRmv2");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("WordGrpRmvMiss"));

        var removeRequest = new RemoveClusterdleWordByGroupNameRequestDto { Word = "nonexistentword" };
        var (response, _, _) = await TechnectionCategoriesTestHelpers.RemoveWordAsync<FailApiResponse>(
            client, "WordGrpRmvMiss", removeRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveWord_FromNonExistentCategory_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("TechWrdRmv3");

        var removeRequest = new RemoveClusterdleWordByGroupNameRequestDto { Word = "commit" };
        var (response, _, _) = await TechnectionCategoriesTestHelpers.RemoveWordAsync<FailApiResponse>(
            client, "NonExistentGroup", removeRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
