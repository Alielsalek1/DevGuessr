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
public class ProgrammingLanguageTagTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task AddTag_ToExistingLanguage_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("AddTagUser");
        var createRequest = new CreateProgrammingLanguageRequestDto
        {
            Name = "TagLang",
            YearFirstAppeared = 2010,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["InitialTag"]
        };

        var (createResponse, createdContent, _) = await ProgrammingLanguagesTestHelpers.CreateAsync<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>(client, createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var name = createdContent!.Data.Name;

        var tagRequest = new AddProgrammingLanguageTagByNameRequestDto { Tag = "NewTag" };
        var (response, content, _) = await ProgrammingLanguagesTestHelpers.AddTagAsync<SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>>(client, name, tagRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Contains("NewTag", content.Data.Tags);
        Assert.Contains("InitialTag", content.Data.Tags);
    }

    [Fact]
    public async Task AddTag_ToNonexistentLanguage_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("AddTagNonexUser");
        var tagRequest = new AddProgrammingLanguageTagByNameRequestDto { Tag = "NewTag" };
        var (response, _, _) = await ProgrammingLanguagesTestHelpers.AddTagAsync<FailApiResponse>(client, "NonExistentLang", tagRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveTag_FromExistingLanguage_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("RemTagUser");
        var createRequest = new CreateProgrammingLanguageRequestDto
        {
            Name = "RemTagLang",
            YearFirstAppeared = 2010,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["TagToRemove", "TagToKeep"]
        };

        var (createResponse, createdContent, _) = await ProgrammingLanguagesTestHelpers.CreateAsync<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>(client, createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var name = createdContent!.Data.Name;

        var tagRequest = new RemoveProgrammingLanguageTagByNameRequestDto { Tag = "TagToRemove" };
        var (response, content, _) = await ProgrammingLanguagesTestHelpers.RemoveTagAsync<SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>>(client, name, tagRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.DoesNotContain("TagToRemove", content.Data.Tags);
        Assert.Contains("TagToKeep", content.Data.Tags);
    }

    [Fact]
    public async Task RemoveTag_FromNonexistentLanguage_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("RemTagNonexUser");
        var tagRequest = new RemoveProgrammingLanguageTagByNameRequestDto { Tag = "Whatever" };
        var (response, _, _) = await ProgrammingLanguagesTestHelpers.RemoveTagAsync<FailApiResponse>(client, "NonExistentLang", tagRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task AddTag_AlreadyExistingTag_Returns409Conflict()
    {
        var client = await GetAuthenticatedClientAsync("AddDupTagUser");
        var createRequest = new CreateProgrammingLanguageRequestDto
        {
            Name = "DupTagLang",
            YearFirstAppeared = 2010,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["ExistingTag"]
        };

        var (createResponse, createdContent, _) = await ProgrammingLanguagesTestHelpers.CreateAsync<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>(client, createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var name = createdContent!.Data.Name;

        var tagRequest = new AddProgrammingLanguageTagByNameRequestDto { Tag = "ExistingTag" };
        var (response, content, _) = await ProgrammingLanguagesTestHelpers.AddTagAsync<FailApiResponse>(client, name, tagRequest);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Equal("The programming language already has this tag.", content.Message);
    }

    [Fact]
    public async Task RemoveTag_NonexistentTag_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("RemMissTagUser");
        var createRequest = new CreateProgrammingLanguageRequestDto
        {
            Name = "RemMissTagLang",
            YearFirstAppeared = 2010,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["ExistingTag"]
        };

        var (createResponse, createdContent, _) = await ProgrammingLanguagesTestHelpers.CreateAsync<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>(client, createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var name = createdContent!.Data.Name;

        var tagRequest = new RemoveProgrammingLanguageTagByNameRequestDto { Tag = "NonExistentTag" };
        var (response, content, _) = await ProgrammingLanguagesTestHelpers.RemoveTagAsync<FailApiResponse>(client, name, tagRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content.Success);
        Assert.Equal("The programming language does not have this tag.", content.Message);
    }
}
