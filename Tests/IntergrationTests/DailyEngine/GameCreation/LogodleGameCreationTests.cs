using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Constants.ApiErrors;
using Application.DTOs.Auth;
using Application.DTOs.LogodlePlayer;
using Application.Utils;
using Domain.Enums;
using Domain.Models.DailyLogodle;
using Domain.Models.LogodleTarget;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.IntergrationTests.DailyEngine.GameCreation;

[Collection("Integration Tests")]
public class LogodleGameCreationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private static LogodleTarget CreateTarget(string name)
    {
        return new LogodleTarget(new LogodleTargetCreationParams
        {
            Name = name,
            ImagePath = $"/uploads/{name}/original.png",
            BlurredImageUrls =
            [
                $"/uploads/{name}/blur_1.png",
                $"/uploads/{name}/blur_2.png",
                $"/uploads/{name}/blur_3.png",
                $"/uploads/{name}/blur_4.png",
                $"/uploads/{name}/blur_5.png"
            ]
        });
    }

    private async Task<HttpClient> GetAdminClientAsync()
    {
        var username = $"la{Guid.NewGuid():N}".Substring(0, 25);
        var email = $"{username.ToLower()}@example.com";
        var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123", Roles.Admin);
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
        return Client;
    }

    private async Task<HttpClient> GetUserClientAsync()
    {
        var username = $"lu{Guid.NewGuid():N}".Substring(0, 25);
        var email = $"{username.ToLower()}@example.com";
        var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123", Roles.User);
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);

        var userClient = new HttpClient(Factory.Server.CreateHandler())
        {
            BaseAddress = Factory.Server.BaseAddress
        };
        userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
        return userClient;
    }

    private async Task SeedTargetsAsync(params LogodleTarget[] targets)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.LogodleTargets.AddRange(targets);
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedPuzzleAsync(DateOnly date, string targetName)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var puzzle = new DailyLogodle(date, targetName);
        dbContext.DailyLogodles.Add(puzzle);
        await dbContext.SaveChangesAsync();
    }

    private async Task<List<DailyLogodle>> GetAllPuzzlesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await dbContext.DailyLogodles
            .AsNoTracking()
            .OrderBy(d => d.PuzzleDate)
            .ToListAsync();
    }

    [Fact]
    public async Task CreateGames_AdminWithTargets_Returns201CreatedWithBatchData()
    {
        var adminClient = await GetAdminClientAsync();
        var target1 = CreateTarget($"Target1_{Guid.NewGuid():N}");
        var target2 = CreateTarget($"Target2_{Guid.NewGuid():N}");
        await SeedTargetsAsync(target1, target2);

        var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.Equal("Logodle puzzles generated successfully.", content.Message);
        Assert.Equal(2, content.Data.CreatedCount);

        var ids = content.Data.Items.Select(i => i.TargetId).ToHashSet();
        Assert.Contains(target1.Id, ids);
        Assert.Contains(target2.Id, ids);
    }

    [Fact]
    public async Task CreateGames_NonAdminUser_Returns403Forbidden()
    {
        var userClient = await GetUserClientAsync();
        var target = CreateTarget($"Target_{Guid.NewGuid():N}");
        await SeedTargetsAsync(target);

        var response = await userClient.PostAsync("/api/v1/logodle/games", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateGames_AnonymousUser_Returns401Unauthorized()
    {
        var target = CreateTarget($"Target_{Guid.NewGuid():N}");
        await SeedTargetsAsync(target);

        var response = await Client.PostAsync("/api/v1/logodle/games", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateGames_EmptyTable_StartsAtUTCTodayAndCreatesOnePerTarget()
    {
        var adminClient = await GetAdminClientAsync();
        var targets = new[]
        {
            CreateTarget($"TargetA_{Guid.NewGuid():N}"),
            CreateTarget($"TargetB_{Guid.NewGuid():N}"),
            CreateTarget($"TargetC_{Guid.NewGuid():N}")
        };
        await SeedTargetsAsync(targets);

        var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(targets.Length, content!.Data.CreatedCount);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedDates = Enumerable.Range(0, targets.Length).Select(i => today.AddDays(i)).ToHashSet();
        var actualDates = content.Data.Items.Select(i => i.PuzzleDate).ToHashSet();
        Assert.Equal(expectedDates, actualDates);

        var stored = await GetAllPuzzlesAsync();
        Assert.Equal(targets.Length, stored.Count);
    }

    [Fact]
    public async Task CreateGames_WithFutureLatestPuzzle_StartsFromLatestPlusOne()
    {
        var adminClient = await GetAdminClientAsync();
        var target1 = CreateTarget($"Target1_{Guid.NewGuid():N}");
        var target2 = CreateTarget($"Target2_{Guid.NewGuid():N}");
        await SeedTargetsAsync(target1, target2);

        var futureLatest = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(8);
        await SeedPuzzleAsync(futureLatest, target1.Name);

        var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);

        var createdDates = content!.Data.Items.Select(i => i.PuzzleDate).OrderBy(d => d).ToList();
        Assert.Equal(futureLatest.AddDays(1), createdDates.First());
        Assert.Equal(futureLatest.AddDays(2), createdDates.Last());
    }

    [Fact]
    public async Task CreateGames_NoTargets_Returns400BadRequest()
    {
        var adminClient = await GetAdminClientAsync();

        var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
        var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content!.Success);
        Assert.Equal(AdminLogodleErrorCodes.NoTargetsFound, content.ErrorCode);
    }

    [Fact]
    public async Task CreateGames_DuplicateDateConcurrentRequest_ReturnsCreatedOrConflict()
    {
        var adminClient1 = await GetAdminClientAsync();
        var adminClient2 = await GetAdminClientAsync();
        var target1 = CreateTarget($"Target1_{Guid.NewGuid():N}");
        var target2 = CreateTarget($"Target2_{Guid.NewGuid():N}");
        await SeedTargetsAsync(target1, target2);

        var task1 = adminClient1.PostAsync("/api/v1/logodle/games", null);
        var task2 = adminClient2.PostAsync("/api/v1/logodle/games", null);

        var responses = await Task.WhenAll(task1, task2);

        Assert.Contains(responses, r => r.StatusCode == HttpStatusCode.Created);
        Assert.Contains(responses, r => r.StatusCode is HttpStatusCode.Created or HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateGames_ItemsMatchDatabaseRows()
    {
        var adminClient = await GetAdminClientAsync();
        var targets = new[]
        {
            CreateTarget($"Target1_{Guid.NewGuid():N}"),
            CreateTarget($"Target2_{Guid.NewGuid():N}"),
            CreateTarget($"Target3_{Guid.NewGuid():N}"),
            CreateTarget($"Target4_{Guid.NewGuid():N}")
        };
        await SeedTargetsAsync(targets);

        var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGamesResponseDto>>();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var stored = await GetAllPuzzlesAsync();
        var byId = stored.ToDictionary(s => s.Id);

        Assert.Equal(content!.Data.CreatedCount, stored.Count);
        foreach (var item in content.Data.Items)
        {
            Assert.True(byId.TryGetValue(item.PuzzleId, out var puzzle));
            var target = targets.Single(t => t.Name == puzzle!.LogodleTargetName);
            Assert.Equal(item.TargetId, target.Id);
            Assert.Equal(item.PuzzleDate, puzzle.PuzzleDate);
        }
    }

    [Fact]
    public async Task CreateGames_WithPastLatestPuzzle_StillStartsAtToday()
    {
        var adminClient = await GetAdminClientAsync();
        var target1 = CreateTarget($"Target1_{Guid.NewGuid():N}");
        var target2 = CreateTarget($"Target2_{Guid.NewGuid():N}");
        await SeedTargetsAsync(target1, target2);

        var pastLatest = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-5);
        await SeedPuzzleAsync(pastLatest, target1.Name);

        var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdDates = content!.Data.Items.Select(i => i.PuzzleDate).OrderBy(d => d).ToList();

        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), createdDates.First());
    }
}
