using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Constants.ApiErrors;
using Application.DTOs.Auth;
using Application.DTOs.LangdlePlayer;
using Application.Utils;
using Domain.Enums;
using Domain.Models.DailyLangdle;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

using LangdleModel = Domain.Models.Langdle.Langdle;
using LangdleCreationParams = Domain.Models.Langdle.LangdleCreationParams;

namespace Tests.IntergrationTests.DailyEngine.GameCreation;

[Collection("Integration Tests")]
public class LangdleGameCreationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private static LangdleModel CreateLanguage(
        int year,
        TypingDiscipline typing,
        TypeStrength strength,
        List<string> tags)
    {
        return new LangdleModel(new LangdleCreationParams
        {
            Name = $"Lang_{Guid.NewGuid():N}",
            YearFirstAppeared = year,
            TypingDiscipline = typing,
            TypeStrength = strength,
            ExecutionModel = ExecutionModel.Compiled,
            MemoryManagement = MemoryManagement.Manual,
            Tags = tags
        });
    }

    private async Task<HttpClient> GetAdminClientAsync()
    {
        var username = $"ta{Guid.NewGuid():N}".Substring(0, 25);
        var email = $"{username.ToLower()}@example.com";
        var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123", Roles.Admin);
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
        return Client;
    }

    private async Task<HttpClient> GetUserClientAsync()
    {
        var username = $"tu{Guid.NewGuid():N}".Substring(0, 25);
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

    private async Task SeedLanguagesAsync(params LangdleModel[] languages)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.LangdleTargets.AddRange(languages);
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedPuzzleAsync(DateOnly date, string languageName)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var puzzle = new DailyLangdle(date, languageName);
        dbContext.DailyLangdles.Add(puzzle);
        await dbContext.SaveChangesAsync();
    }

    private async Task<List<DailyLangdle>> GetAllPuzzlesAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await dbContext.DailyLangdles
            .AsNoTracking()
            .OrderBy(d => d.PuzzleDate)
            .ToListAsync();
    }

    [Fact]
    public async Task CreateGames_AdminWithLanguages_Returns201CreatedWithBatchData()
    {
        var adminClient = await GetAdminClientAsync();
        var lang1 = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
        var lang2 = CreateLanguage(1995, TypingDiscipline.Dynamic, TypeStrength.Weak, ["scripting"]);
        await SeedLanguagesAsync(lang1, lang2);

        var response = await adminClient.PostAsync("/api/v1/langdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLangdleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.Equal("Langdle puzzles generated successfully.", content.Message);
        Assert.Equal(2, content.Data.CreatedCount);

        var ids = content.Data.Items.Select(i => i.TargetId).ToHashSet();
        Assert.Contains(lang1.Id, ids);
        Assert.Contains(lang2.Id, ids);
    }

    [Fact]
    public async Task CreateGames_NonAdminUser_Returns403Forbidden()
    {
        var userClient = await GetUserClientAsync();
        var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
        await SeedLanguagesAsync(language);

        var response = await userClient.PostAsync("/api/v1/langdle/games", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateGames_AnonymousUser_Returns401Unauthorized()
    {
        var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
        await SeedLanguagesAsync(language);

        var response = await Client.PostAsync("/api/v1/langdle/games", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateGames_EmptyTable_StartsAtUTCTodayAndCreatesOnePerLanguage()
    {
        var adminClient = await GetAdminClientAsync();
        var languages = new[]
        {
            CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]),
            CreateLanguage(2001, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]),
            CreateLanguage(2002, TypingDiscipline.Static, TypeStrength.Strong, ["systems"])
        };
        await SeedLanguagesAsync(languages);

        var response = await adminClient.PostAsync("/api/v1/langdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLangdleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(languages.Length, content!.Data.CreatedCount);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedDates = Enumerable.Range(0, languages.Length).Select(i => today.AddDays(i)).ToHashSet();
        var actualDates = content.Data.Items.Select(i => i.PuzzleDate).ToHashSet();
        Assert.Equal(expectedDates, actualDates);

        var stored = await GetAllPuzzlesAsync();
        Assert.Equal(languages.Length, stored.Count);
    }

    [Fact]
    public async Task CreateGames_WithFutureLatestPuzzle_StartsFromLatestPlusOne()
    {
        var adminClient = await GetAdminClientAsync();
        var lang1 = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
        var lang2 = CreateLanguage(2001, TypingDiscipline.Dynamic, TypeStrength.Strong, ["systems"]);
        await SeedLanguagesAsync(lang1, lang2);

        var futureLatest = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10);
        await SeedPuzzleAsync(futureLatest, lang1.Name);

        var response = await adminClient.PostAsync("/api/v1/langdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLangdleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(content);

        var createdDates = content!.Data.Items.Select(i => i.PuzzleDate).OrderBy(d => d).ToList();
        Assert.Equal(futureLatest.AddDays(1), createdDates.First());
        Assert.Equal(futureLatest.AddDays(2), createdDates.Last());
    }

    [Fact]
    public async Task CreateGames_NoLanguages_Returns400BadRequest()
    {
        var adminClient = await GetAdminClientAsync();

        var response = await adminClient.PostAsync("/api/v1/langdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content!.Success);
        Assert.Equal(AdminLangdleErrorCodes.NoLanguagesFound, content.ErrorCode);
    }

    [Fact]
    public async Task CreateGames_DuplicateDateConcurrentRequest_ReturnsCreatedOrConflict()
    {
        var lang1 = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
        var lang2 = CreateLanguage(2001, TypingDiscipline.Dynamic, TypeStrength.Strong, ["systems"]);
        await SeedLanguagesAsync(lang1, lang2);

        var clients = new List<HttpClient>();
        for (var i = 0; i < 8; i++)
        {
            clients.Add(await GetAdminClientAsync());
        }

        var tasks = clients.Select(c => c.PostAsync("/api/v1/langdle/games", null));
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
        var languages = new[]
        {
            CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]),
            CreateLanguage(2001, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]),
            CreateLanguage(2002, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]),
            CreateLanguage(2003, TypingDiscipline.Static, TypeStrength.Strong, ["systems"])
        };
        await SeedLanguagesAsync(languages);

        var response = await adminClient.PostAsync("/api/v1/langdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLangdleGamesResponseDto>>();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var stored = await GetAllPuzzlesAsync();
        var byId = stored.ToDictionary(s => s.Id);

        Assert.Equal(content!.Data.CreatedCount, stored.Count);
        foreach (var item in content.Data.Items)
        {
            Assert.True(byId.TryGetValue(item.PuzzleId, out var puzzle));
            var language = languages.Single(l => l.Name == puzzle!.LangdleName);
            Assert.Equal(item.TargetId, language.Id);
            Assert.Equal(item.PuzzleDate, puzzle.PuzzleDate);
        }
    }

    [Fact]
    public async Task CreateGames_WithPastLatestPuzzle_StillStartsAtToday()
    {
        var adminClient = await GetAdminClientAsync();
        var lang1 = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
        var lang2 = CreateLanguage(2001, TypingDiscipline.Dynamic, TypeStrength.Strong, ["systems"]);
        await SeedLanguagesAsync(lang1, lang2);

        var pastLatest = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-5);
        await SeedPuzzleAsync(pastLatest, lang1.Name);

        var response = await adminClient.PostAsync("/api/v1/langdle/games", null);
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLangdleGamesResponseDto>>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdDates = content!.Data.Items.Select(i => i.PuzzleDate).OrderBy(d => d).ToList();

        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), createdDates.First());
    }
}
