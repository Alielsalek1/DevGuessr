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
public class GetProgrammingLanguageTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task GetByName_ForExistingLanguage_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("GetExistingUser");
        var createRequest = new CreateProgrammingLanguageRequestDto
        {
            Name = "ExistingLang",
            YearFirstAppeared = 2020,
            TypingDiscipline = "Dynamic",
            TypeStrength = "Weak",
            ExecutionModel = "Interpreted",
            MemoryManagement = "GarbageCollected",
            Tags = ["Scripting"]
        };

        var (createResponse, _, _) = await ProgrammingLanguagesTestHelpers.CreateAsync<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>(client, createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        Client.DefaultRequestHeaders.Authorization = null; // Can be anonymous
        var (response, content, _) = await ProgrammingLanguagesTestHelpers.GetByNameAsync<SuccessApiResponse<GetProgrammingLanguageByNameResponseDto>>(Client, "ExistingLang");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal("ExistingLang", content.Data.Name);
    }

    [Fact]
    public async Task GetByName_ForNonexistentLanguage_Returns404NotFound()
    {
        Client.DefaultRequestHeaders.Authorization = null; // Can be anonymous
        var (response, content, _) = await ProgrammingLanguagesTestHelpers.GetByNameAsync<FailApiResponse>(Client, "NonexistentLang");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
    }

    [Fact]
    public async Task GetPaged_ReturnsPaginatedLanguages()
    {
        var client = await GetAuthenticatedClientAsync("GetPagedUser");
        for(int i = 0; i < 3; i++)
        {
            await ProgrammingLanguagesTestHelpers.CreateAsync<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>(client, new CreateProgrammingLanguageRequestDto
            {
                Name = $"PagedLang{i}",
                YearFirstAppeared = 2000,
                TypingDiscipline = "Static",
                TypeStrength = "Strong",
                ExecutionModel = "Compiled",
                MemoryManagement = "Manual",
                Tags = ["PagedTag"]
            });
        }

        Client.DefaultRequestHeaders.Authorization = null;
        var (response, content, _) = await ProgrammingLanguagesTestHelpers.GetPagedAsync<SuccessApiResponse<GetPagedProgrammingLanguagesResponseDto>>(Client, 1, 2);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal(2, content.Data.Items.Count);
        Assert.True(content.Data.TotalCount >= 3);
    }
}
