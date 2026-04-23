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
using Microsoft.Extensions.DependencyInjection;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.IntergrationTests.MythdlePlayer;

[Collection("Integration Tests")]
public class MythdlePlayerTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private static MythdleTarget CreateTarget(string name, bool isFake = false)
    {
        return new MythdleTarget(new MythdleTargetCreationParams
        {
            Name = name,
            Category = "Creature",
            IsFake = isFake,
            Description = $"Description for {name}",
            Difficulty = MythdleDifficulty.Easy
        });
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync(string username)
    {
        var email = $"{username.ToLower()}@example.com";
        var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123", Roles.User);
        var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
        var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
        return Client;
    }

    private async Task<(Guid PuzzleId, string MythTargetName, DateOnly PuzzleDate)> SeedPuzzleAsync(DateOnly? puzzleDate = null)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var actualTargets = new[]
        {
            CreateTarget($"Tech_{Guid.NewGuid():N}"),
            CreateTarget($"Tech_{Guid.NewGuid():N}"),
            CreateTarget($"Tech_{Guid.NewGuid():N}"),
            CreateTarget($"Tech_{Guid.NewGuid():N}")
        };
        var mythTarget = CreateTarget($"Myth_{Guid.NewGuid():N}", isFake: true);

        dbContext.MythdleTargets.AddRange(actualTargets.Append(mythTarget));

        var resolvedPuzzleDate = puzzleDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var selectedTargets = actualTargets.Select(target => target.Name).Append(mythTarget.Name).ToList();
        var puzzle = new DailyMythdle(resolvedPuzzleDate, mythTarget.Name, selectedTargets);
        dbContext.DailyMythdles.Add(puzzle);

        await dbContext.SaveChangesAsync();

        return (puzzle.Id, mythTarget.Name, resolvedPuzzleDate);
    }

    [Fact]
    public async Task Guess_WithCorrectName_Returns200OkAndIsCorrectTrue()
    {
        var client = await GetAuthenticatedClientAsync("MythdleCorrectUser");
        var (puzzleId, mythTargetName, _) = await SeedPuzzleAsync();

        var request = new MythdleGuessRequestDto
        {
            PuzzleId = puzzleId,
            GuessedTargetName = mythTargetName
        };

        var (response, content, _) = await MythdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<MythdleGuessResultDto>>(client, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content!.Data.IsCorrect);
        Assert.Equal(mythTargetName, content.Data.TargetName);
    }

    [Fact]
    public async Task Guess_WithWrongName_Returns200OkAndIsCorrectFalse()
    {
        var client = await GetAuthenticatedClientAsync("MythdleWrongUser");
        var (puzzleId, mythTargetName, _) = await SeedPuzzleAsync();

        var request = new MythdleGuessRequestDto
        {
            PuzzleId = puzzleId,
            GuessedTargetName = $"{mythTargetName}_wrong"
        };

        var (response, content, _) = await MythdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<MythdleGuessResultDto>>(client, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.False(content!.Data.IsCorrect);
        Assert.Null(content.Data.TargetName);
    }

    [Fact]
    public async Task Guess_WithCaseInsensitiveMatch_Returns200OkAndIsCorrectTrue()
    {
        var client = await GetAuthenticatedClientAsync("MythdleCaseUser");
        var (puzzleId, mythTargetName, _) = await SeedPuzzleAsync();

        var request = new MythdleGuessRequestDto
        {
            PuzzleId = puzzleId,
            GuessedTargetName = mythTargetName.ToUpperInvariant()
        };

        var (response, content, _) = await MythdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<MythdleGuessResultDto>>(client, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content!.Data.IsCorrect);
    }

    [Fact]
    public async Task Guess_WithInvalidPuzzleId_Returns400BadRequest()
    {
        var client = await GetAuthenticatedClientAsync("MythdleInvalidPuzzleUser");
        await SeedPuzzleAsync();

        var request = new MythdleGuessRequestDto
        {
            PuzzleId = Guid.NewGuid(),
            GuessedTargetName = "anything"
        };

        var (response, content, _) = await MythdlePlayerTestHelpers.PostGuessAsync<FailApiResponse>(client, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(MythdlePlayerErrorCodes.InvalidPuzzleId, content!.ErrorCode);
    }

    [Fact]
    public async Task Guess_WithoutAccessToken_AllowsAnonymousGuess()
    {
        var (puzzleId, mythTargetName, _) = await SeedPuzzleAsync();

        var request = new MythdleGuessRequestDto
        {
            PuzzleId = puzzleId,
            GuessedTargetName = mythTargetName
        };

        var (response, content, _) = await MythdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<MythdleGuessResultDto>>(Client, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content!.Data.IsCorrect);
    }

    [Fact]
    public async Task GetGameByDate_WithExistingPuzzle_Returns200OkWithPuzzleId()
    {
        var (puzzleId, mythTargetName, puzzleDate) = await SeedPuzzleAsync(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1));

        var response = await Client.GetAsync($"/api/v1/mythdle/games/by-date?date={puzzleDate:yyyy-MM-dd}");
        var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<MythdleGameDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(puzzleId, content!.Data.PuzzleId);
        Assert.Equal(puzzleDate, content.Data.PuzzleDate);
        Assert.Equal(5, content.Data.Targets.Count);
        Assert.Contains(content.Data.Targets, target => target.Name == mythTargetName);
    }

    [Fact]
    public async Task GetGameByDate_WithoutPuzzleForDate_Returns404NotFound()
    {
        var missingDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-45);

        var response = await Client.GetAsync($"/api/v1/mythdle/games/by-date?date={missingDate:yyyy-MM-dd}");
        var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(MythdlePlayerErrorCodes.GameNotFoundForDate, content!.ErrorCode);
    }
}
