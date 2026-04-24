using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Constants.ApiErrors;
using Application.DTOs.Auth;
using Application.DTOs.MythdlePlayer;
using Application.Utils;
using Domain.Enums;
using Domain.Models.DailyMythdle;
using Domain.Models.MythdleTarget;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.IntergrationTests.DailyEngine.GameCreation;

[Collection("Integration Tests")]
public class MythdleGameCreationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private static MythdleTarget CreateTarget(string name, string category = "Creature", bool isFake = false, MythdleDifficulty difficulty = MythdleDifficulty.Easy)
    {
        return new MythdleTarget(new MythdleTargetCreationParams
        {
            Name = name,
            Category = category,
            IsFake = isFake,
            Description = $"Description for {name}",
            Difficulty = difficulty
        });
    }

    private async Task<HttpClient> GetAdminClientAsync()
    {
        var username = $"ma{Guid.NewGuid():N}".Substring(0, 25);
        var email = $"{username.ToLower()}@example.com";
        var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123", Roles.Admin);
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
        return Client;
    }

    private async Task<HttpClient> GetUserClientAsync()
    {
        var username = $"mu{Guid.NewGuid():N}".Substring(0, 25);
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

    private async Task SeedTargetsAsync(params MythdleTarget[] targets)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.MythdleTargets.AddRange(targets);
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedPuzzleAsync(DateOnly date, string targetName)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var puzzle = new DailyMythdle(date, targetName, [targetName]);
        dbContext.DailyMythdles.Add(puzzle);
        await dbContext.SaveChangesAsync();
    }

    private async Task<List<DailyMythdle>> GetAllPuzzlesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await dbContext.DailyMythdles
            .AsNoTracking()
            .OrderBy(d => d.PuzzleDate)
            .ToListAsync();
    }

    [Fact]
    public async Task CreateGames_AdminWithTargets_Returns201CreatedWithBatchData()
    {
        var adminClient = await GetAdminClientAsync();
        var easyTargets = new[]
        {
            CreateTarget($"Easy_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Easy),
            CreateTarget($"Easy_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Easy)
        };
        var mediumTargets = new[]
        {
            CreateTarget($"Med_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Medium),
            CreateTarget($"Med_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Medium)
        };
        var hardTarget = CreateTarget($"Hard_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Hard);
        var mythTarget = CreateTarget($"Myth_{Guid.NewGuid():N}", "Legend", true);
        
        await SeedTargetsAsync(easyTargets.Concat(mediumTargets).Append(hardTarget).Append(mythTarget).ToArray());

        var response = await adminClient.PostAsync("/api/v1/mythdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateMythdleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.Equal("Mythdle puzzles generated successfully.", content.Message);
        Assert.Equal(1, content.Data.CreatedCount);

        var createdGame = content.Data.Items.Single();
        Assert.Equal(6, createdGame.Targets.Count);
        Assert.Equal(1, createdGame.Targets.Count(target => target.IsFake));
        Assert.Contains(createdGame.Targets, target => target.Name == mythTarget.Name && target.IsFake);
    }

    [Fact]
    public async Task CreateGames_NonAdminUser_Returns403Forbidden()
    {
        var userClient = await GetUserClientAsync();
        var target = CreateTarget($"Myth_{Guid.NewGuid():N}");
        await SeedTargetsAsync(target);

        var response = await userClient.PostAsync("/api/v1/mythdle/games", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateGames_AnonymousUser_Returns401Unauthorized()
    {
        var target = CreateTarget($"Myth_{Guid.NewGuid():N}");
        await SeedTargetsAsync(target);

        var response = await Client.PostAsync("/api/v1/mythdle/games", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateGames_NoTargets_Returns400BadRequest()
    {
        var adminClient = await GetAdminClientAsync();

        var response = await adminClient.PostAsync("/api/v1/mythdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content!.Success);
        Assert.Equal(AdminMythdleErrorCodes.NoTargetsFound, content.ErrorCode);
    }

    private async Task SeedTargetSetAsync(int count)
    {
        // For game creation we need 1 hard, 2 medium, 2 easy, 1 myth per day
        var neededTargets = new List<MythdleTarget>();
        for (var i = 0; i < count; i++)
        {
            neededTargets.Add(CreateTarget($"Hard_{i}_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Hard));
            neededTargets.Add(CreateTarget($"Med1_{i}_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Medium));
            neededTargets.Add(CreateTarget($"Med2_{i}_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Medium));
            neededTargets.Add(CreateTarget($"Easy1_{i}_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Easy));
            neededTargets.Add(CreateTarget($"Easy2_{i}_{Guid.NewGuid():N}", difficulty: MythdleDifficulty.Easy));
            neededTargets.Add(CreateTarget($"Myth_{i}_{Guid.NewGuid():N}", isFake: true));
        }
        await SeedTargetsAsync(neededTargets.ToArray());
    }

    [Fact]
    public async Task CreateGames_WithFutureLatestPuzzle_StartsFromLatestPlusOne()
    {
        var adminClient = await GetAdminClientAsync();
        await SeedTargetSetAsync(2);

        var mythTarget = CreateTarget($"ExistingMyth_{Guid.NewGuid():N}", isFake: true);
        await SeedTargetsAsync(mythTarget);

        var futureLatest = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(12);
        await SeedPuzzleAsync(futureLatest, mythTarget.Name);

        var response = await adminClient.PostAsync("/api/v1/mythdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateMythdleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);

        var createdDates = content!.Data.Items.Select(i => i.PuzzleDate).OrderBy(d => d).ToList();
        Assert.Equal(2, createdDates.Count);
        Assert.Equal(futureLatest.AddDays(1), createdDates.First());
        Assert.Equal(futureLatest.AddDays(2), createdDates.Last());
    }

    [Fact]
    public async Task CreateGames_DuplicateDateConcurrentRequest_ReturnsCreatedOrConflict()
    {
        await SeedTargetSetAsync(1);

        var clients = new List<HttpClient>();
        for (var i = 0; i < 8; i++)
        {
            clients.Add(await GetAdminClientAsync());
        }

        var tasks = clients.Select(c => c.PostAsync("/api/v1/mythdle/games", null));
        var responses = await Task.WhenAll(tasks);

        var createdCount = responses.Count(r => r.StatusCode == HttpStatusCode.Created);
        Assert.True(createdCount > 0, "At least one batch request should create puzzles.");

        Assert.All(
            responses,
            r => Assert.True(
                r.StatusCode == HttpStatusCode.Created || r.StatusCode == HttpStatusCode.Conflict,
                $"Unexpected status code: {r.StatusCode}"));
    }

    [Fact]
    public async Task CreateGames_ItemsMatchDatabaseRows()
    {
        var adminClient = await GetAdminClientAsync();
        await SeedTargetSetAsync(1);

        var response = await adminClient.PostAsync("/api/v1/mythdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateMythdleGamesResponseDto>>();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var stored = await GetAllPuzzlesAsync();
        var byId = stored.ToDictionary(s => s.Id);

        Assert.Equal(content!.Data.CreatedCount, stored.Count);
        foreach (var item in content.Data.Items)
        {
            Assert.True(byId.TryGetValue(item.PuzzleId, out var puzzle));
            Assert.Equal(6, item.Targets.Count);
            Assert.Equal(1, item.Targets.Count(target => target.IsFake));
            Assert.Contains(item.Targets, target => target.Name == puzzle!.MythdleTargetName && target.IsFake);
            Assert.Equal(item.PuzzleDate, puzzle.PuzzleDate);
        }
    }
}
