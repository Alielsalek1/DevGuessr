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
public class DeleteLangdleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task Delete_ExistingLanguage_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("DeleteUser");
        var createRequest = new CreateLangdleRequestDto
        {
            Name = "DeleteTargetLang",
            YearFirstAppeared = 1995,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["DeleteTag"]
        };

        var (createResponse, createdContent, _) = await LangdleTestHelpers.CreateAsync<SuccessApiResponse<CreateLangdleResponseDto>>(client, createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var name = createdContent!.Data.Name;

        var (response, content, _) = await LangdleTestHelpers.DeleteAsync<SuccessApiResponse>(client, name);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);

        var (getResponse, _, _) = await LangdleTestHelpers.GetByNameAsync<FailApiResponse>(client, "DeleteTargetLang");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NonexistentLanguage_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("DeleteNonexUser");
        var (response, _, _) = await LangdleTestHelpers.DeleteAsync<FailApiResponse>(client, "NonExistentLang");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
