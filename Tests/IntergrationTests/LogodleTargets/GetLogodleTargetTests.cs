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
public class GetLogodleTargetTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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
    public async Task GetPaged_Returns200OkWithData()
    {
        var client = await GetAuthenticatedClientAsync("LgdlGet1");
        
        // Ensure at least one exists
        await LogodleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateLogodleTargetResponseDto>>(
            client, 
            name: "GetPagedTarget", 
            imageBytes: LogodleTargetsTestHelpers.ValidPngImage);

        var (response, content, _) = await LogodleTargetsTestHelpers.GetPagedAsync<SuccessApiResponse<GetPagedLogodleTargetsResponseDto>>(
            client, 
            pageNumber: 1, 
            pageSize: 10);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content?.Data);
        Assert.True(content.Data.Items.Count() > 0);
    }

    [Fact]
    public async Task GetByName_WithValidName_Returns200Ok()
    {
        var client = await GetAuthenticatedClientAsync("LgdlGet2");
        var targetName = "GetByNameTarget";

        await LogodleTargetsTestHelpers.CreateAsync<SuccessApiResponse<CreateLogodleTargetResponseDto>>(
            client, 
            name: targetName, 
            imageBytes: LogodleTargetsTestHelpers.ValidPngImage
        );

        var (response, content, _) = await LogodleTargetsTestHelpers.GetByNameAsync<SuccessApiResponse<GetLogodleTargetByNameResponseDto>>(
            client, 
            targetName
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content?.Data);
        Assert.Equal(targetName, content.Data.Name);
    }

    [Fact]
    public async Task GetByName_WithInvalidName_Returns404NotFound()
    {
        var client = await GetAuthenticatedClientAsync("LgdlGet3");

        var (response, _, _) = await LogodleTargetsTestHelpers.GetByNameAsync<FailApiResponse>(
            client, 
            "NonExistentTargetABC");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
