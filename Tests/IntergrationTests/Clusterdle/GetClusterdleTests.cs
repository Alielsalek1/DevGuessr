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
public class GetClusterdleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task GetByGroupName_WithExistingCategory_Returns200OK()
    {
        var client = await GetAuthenticatedClientAsync("TechGetUsr1");
        var createRequest = TechnectionCategoriesTestHelpers.BuildValidRequest("GetGroup1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(client, createRequest);

        var (response, content, json) = await TechnectionCategoriesTestHelpers.GetByGroupNameAsync<SuccessApiResponse<GetClusterdleByGroupNameResponseDto>>(client, "GetGroup1");

        Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.Equal("GetGroup1", content.Data.GroupName);
    }

    [Fact]
    public async Task GetByGroupName_WithNonExistentCategory_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("TechGetUsr2");

        var (response, _, _) = await TechnectionCategoriesTestHelpers.GetByGroupNameAsync<FailApiResponse>(client, "NonExistentGroup");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPaged_Returns200OK()
    {
        var client = await GetAuthenticatedClientAsync("TechGetPgd1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateClusterdleResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("PagedGroup1"));

        var (response, content, json) = await TechnectionCategoriesTestHelpers.GetPagedAsync<SuccessApiResponse<GetPagedClusterdleResponseDto>>(client);

        Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.True(content.Success);
    }
}
