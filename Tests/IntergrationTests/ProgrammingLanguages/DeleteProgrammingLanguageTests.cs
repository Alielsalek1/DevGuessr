using System.Net;
using System.Net.Http.Headers;
using Application.DTOs.Auth;
using Application.DTOs.ProgrammingLanguage;
using Application.Utils;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.ProgrammingLanguages;

[Collection("Integration Tests")]
public class DeleteProgrammingLanguageTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<HttpClient> GetAuthenticatedClientAsync(string username)
    {
        var email = $"{username.ToLower()}@example.com";
        var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123");
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);
        
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
        return Client;
    }

    [Fact]
    public async Task Delete_ExistingLanguage_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("DeleteUser");
        var createRequest = new CreateProgrammingLanguageRequestDto
        {
            Name = "DeleteTargetLang",
            YearFirstAppeared = 1995,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["DeleteTag"]
        };

        var (createResponse, createdContent, _) = await ProgrammingLanguagesTestHelpers.CreateAsync<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>(client, createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var name = createdContent!.Data.Name;

        var (response, content, _) = await ProgrammingLanguagesTestHelpers.DeleteAsync<SuccessApiResponse>(client, name);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);

        Client.DefaultRequestHeaders.Authorization = null;
        var (getResponse, _, _) = await ProgrammingLanguagesTestHelpers.GetByNameAsync<FailApiResponse>(Client, "DeleteTargetLang");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NonexistentLanguage_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("DeleteNonexUser");
        var (response, _, _) = await ProgrammingLanguagesTestHelpers.DeleteAsync<FailApiResponse>(client, "NonExistentLang");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
