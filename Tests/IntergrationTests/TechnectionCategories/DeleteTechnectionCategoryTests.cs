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
public class DeleteTechnectionCategoryTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task Delete_WithExistingCategory_Returns200OK()
    {
        var client = await GetAuthenticatedClientAsync("TechDelUsr1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateTechnectionCategoryResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("DelGroup1"));

        var (response, content, json) = await TechnectionCategoriesTestHelpers.DeleteAsync<SuccessApiResponse>(client, "DelGroup1");

        Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.True(content.Success);
    }

    [Fact]
    public async Task Delete_WithNonExistentCategory_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("TechDelUsr2");

        var (response, _, _) = await TechnectionCategoriesTestHelpers.DeleteAsync<FailApiResponse>(client, "NonExistentForDel");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ThenGet_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("TechDelGet1");
        await TechnectionCategoriesTestHelpers.CreateAsync<SuccessApiResponse<CreateTechnectionCategoryResponseDto>>(
            client, TechnectionCategoriesTestHelpers.BuildValidRequest("DelThenGet"));

        await TechnectionCategoriesTestHelpers.DeleteAsync<SuccessApiResponse>(client, "DelThenGet");

        var (response, _, _) = await TechnectionCategoriesTestHelpers.GetByGroupNameAsync<FailApiResponse>(client, "DelThenGet");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
