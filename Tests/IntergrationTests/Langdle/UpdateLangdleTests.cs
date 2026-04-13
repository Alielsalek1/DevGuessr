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
public class UpdateLangdleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task Update_WithValidData_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("UpdateUser");
        var createRequest = new CreateLangdleRequestDto
        {
            Name = "UpdatableLang",
            YearFirstAppeared = 2000,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["UpdateTag"]
        };

        var (createResponse, createdContent, _) = await LangdleTestHelpers.CreateAsync<SuccessApiResponse<CreateLangdleResponseDto>>(client, createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var name = createdContent!.Data.Name;

        var updateRequest = new UpdateLangdleByNameRequestDto
        {
            Name = "UpdatedLangName",
            YearFirstAppeared = 2005,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual"
        };

        var (response, content, _) = await LangdleTestHelpers.UpdateAsync<SuccessApiResponse<UpdateLangdleByNameResponseDto>>(client, name, updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Success);
        Assert.Equal("UpdatedLangName", content.Data.Name);
        Assert.Equal(2005, content.Data.YearFirstAppeared);
    }

    [Fact]
    public async Task Update_ForNonexistentLanguage_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("UpdateNonexUser");
        var updateRequest = new UpdateLangdleByNameRequestDto
        {
            Name = "RandomLangName",
            YearFirstAppeared = 2005,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual"
        };

        var (response, _, _) = await LangdleTestHelpers.UpdateAsync<FailApiResponse>(client, "NonExistentLang", updateRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ToDuplicateName_Returns409Conflict()
    {
        var client = await GetAuthenticatedClientAsync("UpdateDupUser");
        var createRequest1 = new CreateLangdleRequestDto
        {
            Name = "FirstLang",
            YearFirstAppeared = 2000,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["UpdateTag1"]
        };
        var createRequest2 = new CreateLangdleRequestDto
        {
            Name = "SecondLang",
            YearFirstAppeared = 2000,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual",
            Tags = ["UpdateTag2"]
        };

        await LangdleTestHelpers.CreateAsync<SuccessApiResponse<CreateLangdleResponseDto>>(client, createRequest1);
        var (createResponse2, createdContent2, _) = await LangdleTestHelpers.CreateAsync<SuccessApiResponse<CreateLangdleResponseDto>>(client, createRequest2);
        Assert.Equal(HttpStatusCode.Created, createResponse2.StatusCode);
        
        var nameToUpdate = createdContent2!.Data.Name;

        var updateRequest = new UpdateLangdleByNameRequestDto
        {
            Name = "FirstLang", // Already taken
            YearFirstAppeared = 2000,
            TypingDiscipline = "Static",
            TypeStrength = "Strong",
            ExecutionModel = "Compiled",
            MemoryManagement = "Manual"
        };

        var (response, _, _) = await LangdleTestHelpers.UpdateAsync<FailApiResponse>(client, nameToUpdate, updateRequest);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
