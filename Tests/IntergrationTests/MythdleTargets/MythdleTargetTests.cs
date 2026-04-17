using System.Net;
using System.Net.Http.Headers;
using Application.DTOs.Auth;
using Application.DTOs.MythdleTarget;
using Application.Utils;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.MythdleTargets;

[Collection("Integration Tests")]
public class MythdleTargetTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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

    private static CreateMythdleTargetDto CreateRequest(string name, bool isFake = false) => new()
    {
        Name = name,
        Category = "Creature",
        IsFake = isFake,
        Description = $"Description for {name}"
    };

    [Fact]
    public async Task Create_WithValidData_Returns201Created()
    {
        var client = await GetAuthenticatedClientAsync("MythCreateUsr");
        var request = CreateRequest($"Myth_{Guid.NewGuid():N}");

        var (response, content, json) = await MythdleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateMythdleTargetResponseDto>>(client, request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.Equal(request.Name, content.Data.Name);
        Assert.Contains("Mythdle target created successfully.", json);
    }

    [Fact]
    public async Task Create_WithDuplicateName_Returns409Conflict()
    {
        var client = await GetAuthenticatedClientAsync("MythDupUsr");
        var request = CreateRequest($"MythDup_{Guid.NewGuid():N}");

        var (initialResponse, _, _) = await MythdleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateMythdleTargetResponseDto>>(client, request);
        Assert.Equal(HttpStatusCode.Created, initialResponse.StatusCode);

        var (response, _, _) = await MythdleTargetsTestHelpers.CreateAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithInvalidData_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("MythInvUsr");
        var request = CreateRequest(string.Empty);

        var (response, _, _) = await MythdleTargetsTestHelpers.CreateAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPaged_Returns200OkWithData()
    {
        var client = await GetAuthenticatedClientAsync("MythGetUsr");
        var request = CreateRequest($"MythPage_{Guid.NewGuid():N}", isFake: true);

        await MythdleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateMythdleTargetResponseDto>>(client, request);

        var (response, content, _) = await MythdleTargetsTestHelpers.GetPagedAsync<SuccessApiResponse<GetPagedMythdleTargetsResponseDto>>(client);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content?.Data);
        Assert.Contains(content!.Data.Targets, target => target.Name == request.Name && target.IsFake == request.IsFake);
    }

    [Fact]
    public async Task Delete_WithValidName_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("MythDelUsr");
        var request = CreateRequest($"MythDel_{Guid.NewGuid():N}");

        await MythdleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateMythdleTargetResponseDto>>(client, request);

        var (response, _, _) = await MythdleTargetsTestHelpers.DeleteAsync<SuccessApiResponse>(client, request.Name);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithInvalidName_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("MythDelMissingUsr");

        var (response, _, _) = await MythdleTargetsTestHelpers.DeleteAsync<FailApiResponse>(client, "MissingMythdleTarget");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}