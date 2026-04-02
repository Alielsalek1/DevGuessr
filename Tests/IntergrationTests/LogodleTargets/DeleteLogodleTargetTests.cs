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
public class DeleteLogodleTargetTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task Delete_WithValidName_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("LgdlDel1");
        var targetName = "DeleteTargetValid";

        await LogodleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateLogodleTargetResponseDto>>(
            client, 
            name: targetName, 
            imageBytes: LogodleTargetsTestHelpers.ValidPngImage);

        var (response, _, _) = await LogodleTargetsTestHelpers.DeleteAsync<SuccessApiResponse>(
            client, 
            targetName);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithInvalidName_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("LgdlDel2");

        var (response, _, _) = await LogodleTargetsTestHelpers.DeleteAsync<FailApiResponse>(
            client, 
            "NonExistentTargetXYZ");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
