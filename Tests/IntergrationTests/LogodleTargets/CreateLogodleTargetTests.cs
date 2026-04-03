using System.Net;
using System.Net.Http.Headers;
using Application.DTOs.Auth;
using Application.DTOs.LogodleTarget;
using Application.Utils;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.LogodleTargets;

[Collection("Integration Tests")]
public class CreateLogodleTargetTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task Create_WithValidData_Returns201Created()
    {
        var client = await GetAuthenticatedClientAsync("LogodleCreateUsr");
        
        var (response, content, json) = await LogodleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateLogodleTargetResponseDto>>(
            client, 
            name: "ValidTarget", 
            imageBytes: LogodleTargetsTestHelpers.ValidPngImage);

        Assert.True(response.StatusCode == HttpStatusCode.Created, $"Expected Created, got {response.StatusCode}. Output: {json}");
        Assert.NotNull(content);
        Assert.True(content.Success);
    }

    [Fact]
    public async Task Create_WithDuplicateName_Returns409Conflict()
    {
        var client = await GetAuthenticatedClientAsync("LgdlDupUsr");
        
        var (initialResponse, _, _) = await LogodleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateLogodleTargetResponseDto>>(
            client, 
            name: "DuplicateTarget", 
            imageBytes: LogodleTargetsTestHelpers.ValidPngImage);
        Assert.Equal(HttpStatusCode.Created, initialResponse.StatusCode);
        
        var (response, _, _) = await LogodleTargetsTestHelpers.CreateAsync<FailApiResponse>(
            client, 
            name: "DuplicateTarget", 
            imageBytes: LogodleTargetsTestHelpers.ValidPngImage);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingImage_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("LgdlInv1");
        
        var (response, _, _) = await LogodleTargetsTestHelpers.CreateAsync<FailApiResponse>(
            client, 
            name: "MissingImageTarget", 
            imageBytes: null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithMissingName_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("LgdlInv2");
        
        var (response, _, _) = await LogodleTargetsTestHelpers.CreateAsync<FailApiResponse>(
            client, 
            name: "", 
            imageBytes: LogodleTargetsTestHelpers.ValidPngImage);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
