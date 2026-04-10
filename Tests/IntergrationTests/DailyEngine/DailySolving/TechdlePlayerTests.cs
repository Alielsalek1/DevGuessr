using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Application.DTOs.TechdlePlayer;
using Application.Utils;
using Domain.Enums;
using Domain.Models.DailyTechdle;
using Domain.Models.ProgrammingLanguage;
using Infrastructure.Persistance;
using Microsoft.Extensions.DependencyInjection;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.IntergrationTests.TechdlePlayer;

[Collection("Integration Tests")]
public class TechdlePlayerTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
	private static ProgrammingLanguage CreateLanguage(
		int year,
		TypingDiscipline typing,
		TypeStrength strength,
		List<string> tags)
	{
		return new ProgrammingLanguage(new ProgrammingLanguageCreationParams
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

	private async Task<Guid> SeedPuzzleAsync(ProgrammingLanguage target, DateOnly? puzzleDate = null, params ProgrammingLanguage[] guesses)
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		dbContext.ProgrammingLanguages.Add(target);
		if (guesses.Length > 0)
		{
			dbContext.ProgrammingLanguages.AddRange(guesses);
		}

		var resolvedPuzzleDate = puzzleDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
		var puzzle = new DailyTechdle(resolvedPuzzleDate, target.Id);
		dbContext.DailyTechdles.Add(puzzle);

		await dbContext.SaveChangesAsync();
		return puzzle.Id;
	}

	private static MatchStatus GetStatus(SuccessApiResponse<TechdleGuessResultDto> content, string attributeName)
	{
		return content.Data.AttributeFeedback.Single(x => x.AttributeName == attributeName).Status;
	}

	private async Task<HttpClient> GetAuthenticatedClientAsync(string username)
	{
		var email = $"{username.ToLower()}@example.com";
		var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123");
		var loginRequest = new LoginRequestDto { UsernameOrEmail = email, Password = password };
		var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);

		Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
		return Client;
	}

	private async Task<(Guid PuzzleId, string TargetLanguageName, string OtherLanguageName)> SeedPuzzleAndLanguagesAsync(DateOnly? puzzleDate = null)
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var targetLanguage = new ProgrammingLanguage(new ProgrammingLanguageCreationParams
		{
			Name = $"TargetLang_{Guid.NewGuid():N}",
			YearFirstAppeared = 1991,
			TypingDiscipline = TypingDiscipline.Static,
			TypeStrength = TypeStrength.Strong,
			ExecutionModel = ExecutionModel.Compiled,
			MemoryManagement = MemoryManagement.Manual,
			Tags = ["systems", "performance"]
		});

		var otherLanguage = new ProgrammingLanguage(new ProgrammingLanguageCreationParams
		{
			Name = $"GuessLang_{Guid.NewGuid():N}",
			YearFirstAppeared = 1995,
			TypingDiscipline = TypingDiscipline.Static,
			TypeStrength = TypeStrength.Weak,
			ExecutionModel = ExecutionModel.Interpreted,
			MemoryManagement = MemoryManagement.GarbageCollected,
			Tags = ["scripting", "performance"]
		});

		dbContext.ProgrammingLanguages.AddRange(targetLanguage, otherLanguage);

		var resolvedPuzzleDate = puzzleDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
		var puzzle = new DailyTechdle(resolvedPuzzleDate, targetLanguage.Id);
		dbContext.DailyTechdles.Add(puzzle);

		await dbContext.SaveChangesAsync();

		return (puzzle.Id, targetLanguage.Name, otherLanguage.Name);
	}

	[Fact]
	public async Task Guess_WithValidPuzzleAndGuess_Returns200OkWithFeedback()
	{
		var client = await GetAuthenticatedClientAsync("TechdleValidUser");
		var (puzzleId, _, otherLanguageName) = await SeedPuzzleAndLanguagesAsync();

		var request = new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = otherLanguageName
		};

		var (response, content, json) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, request);

		Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Output: {json}");
		Assert.NotNull(content);
		Assert.True(content!.Success);
		Assert.False(content.Data.IsCorrect);
		Assert.Equal(3, content.Data.AttributeFeedback.Count);
		Assert.Contains(content.Data.AttributeFeedback, x => x.AttributeName == "ReleaseYear");
		Assert.Contains(content.Data.AttributeFeedback, x => x.AttributeName == "TypingType");
		Assert.Contains(content.Data.AttributeFeedback, x => x.AttributeName == "Tags");
	}

	[Fact]
	public async Task Guess_WithCorrectLanguage_ReturnsIsCorrectTrue()
	{
		var client = await GetAuthenticatedClientAsync("TechdleCorrectUser");
		var (puzzleId, targetLanguageName, _) = await SeedPuzzleAndLanguagesAsync();

		var request = new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = targetLanguageName
		};

		var (response, content, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(content!.Data.IsCorrect);
	}

	[Fact]
	public async Task Guess_WithDifferentLanguageNameCase_ReturnsIsCorrectTrue()
	{
		var client = await GetAuthenticatedClientAsync("TechCaseUser");
		var (puzzleId, targetLanguageName, _) = await SeedPuzzleAndLanguagesAsync();

		var request = new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = targetLanguageName.ToUpperInvariant()
		};

		var (response, content, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(content!.Data.IsCorrect);
	}

	[Fact]
	public async Task GetGameByDate_WithExistingPuzzle_Returns200OkWithPuzzleId()
	{
		var puzzleDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2);
		var (puzzleId, _, _) = await SeedPuzzleAndLanguagesAsync(puzzleDate);

		var response = await Client.GetAsync($"/api/v1/techdle/games/by-date?date={puzzleDate:yyyy-MM-dd}");
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<TechdleGameDto>>();

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.Equal(puzzleId, content!.Data.PuzzleId);
		Assert.Equal(puzzleDate, content.Data.PuzzleDate);
	}

	[Fact]
	public async Task GetGameByDate_WithoutPuzzleForDate_Returns404NotFound()
	{
		var missingDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-30);

		var response = await Client.GetAsync($"/api/v1/techdle/games/by-date?date={missingDate:yyyy-MM-dd}");
		var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		Assert.NotNull(content);
		Assert.Equal("ERR_PUZZLE_NOT_FOUND_FOR_DATE", content!.ErrorCode);
	}

	[Fact]
	public async Task Guess_WithHistoricalPuzzleId_Returns200Ok()
	{
		var client = await GetAuthenticatedClientAsync("TechHistUser");
		var historicalDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
		var (puzzleId, targetLanguageName, _) = await SeedPuzzleAndLanguagesAsync(historicalDate);

		var request = new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = targetLanguageName
		};

		var (response, content, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(content!.Data.IsCorrect);
	}

	[Fact]
	public async Task Guess_WithInvalidPuzzleId_Returns400BadRequest()
	{
		var client = await GetAuthenticatedClientAsync("TechdleInvalidPuzzleUser");
		var (_, _, otherLanguageName) = await SeedPuzzleAndLanguagesAsync();

		var request = new TechdleGuessRequestDto
		{
			PuzzleId = Guid.NewGuid(),
			GuessedLanguageName = otherLanguageName
		};

		var (response, content, _) = await TechdlePlayerTestHelpers.PostGuessAsync<FailApiResponse>(client, request);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Success);
		Assert.Equal("ERR_INVALID_PUZZLE_ID", content.ErrorCode);
	}

	[Fact]
	public async Task Guess_WithUnknownGuessedLanguage_Returns404NotFound()
	{
		var client = await GetAuthenticatedClientAsync("TechdleUnknownGuessUser");
		var (puzzleId, _, _) = await SeedPuzzleAndLanguagesAsync();

		var request = new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = $"UnknownLang_{Guid.NewGuid():N}"
		};

		var (response, content, _) = await TechdlePlayerTestHelpers.PostGuessAsync<FailApiResponse>(client, request);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Success);
		Assert.Equal("ERR_GUESSED_LANGUAGE_NOT_FOUND", content.ErrorCode);
	}

	[Fact]
	public async Task Guess_WithoutAccessToken_AllowsAnonymousGuess()
	{
		var (puzzleId, _, otherLanguageName) = await SeedPuzzleAndLanguagesAsync();

		var request = new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = otherLanguageName
		};

		var (response, content, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(Client, request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Data.IsCorrect);
	}

	[Fact]
	public async Task Guess_ReleaseYearFeedback_CoversHigherLowerAndMatch()
	{
		var client = await GetAuthenticatedClientAsync("TechdleYearStatusUser");
		var target = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["a", "b"]);
		var lowerGuess = CreateLanguage(1990, TypingDiscipline.Static, TypeStrength.Strong, ["a", "b"]);
		var higherGuess = CreateLanguage(2010, TypingDiscipline.Static, TypeStrength.Strong, ["a", "b"]);

		var puzzleId = await SeedPuzzleAsync(target, null, lowerGuess, higherGuess);

		var (_, lowerContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = lowerGuess.Name
		});

		var (_, higherContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = higherGuess.Name
		});

		var (_, matchContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = target.Name
		});

		Assert.NotNull(lowerContent);
		Assert.NotNull(higherContent);
		Assert.NotNull(matchContent);
		Assert.Equal(MatchStatus.Higher, GetStatus(lowerContent!, "ReleaseYear"));
		Assert.Equal(MatchStatus.Lower, GetStatus(higherContent!, "ReleaseYear"));
		Assert.Equal(MatchStatus.Match, GetStatus(matchContent!, "ReleaseYear"));
	}

	[Fact]
	public async Task Guess_TypingTypeFeedback_CoversGreenYellowAndMiss()
	{
		var client = await GetAuthenticatedClientAsync("TechdleTypingStatusUser");
		var target = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["a"]);
		var partialGuess = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Weak, ["a"]);
		var missGuess = CreateLanguage(2000, TypingDiscipline.Dynamic, TypeStrength.Weak, ["a"]);

		var puzzleId = await SeedPuzzleAsync(target, null, partialGuess, missGuess);

		var (_, partialContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = partialGuess.Name
		});

		var (_, missContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = missGuess.Name
		});

		var (_, greenContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = target.Name
		});

		Assert.NotNull(partialContent);
		Assert.NotNull(missContent);
		Assert.NotNull(greenContent);
		Assert.Equal(MatchStatus.Partial, GetStatus(partialContent!, "TypingType"));
		Assert.Equal(MatchStatus.Miss, GetStatus(missContent!, "TypingType"));
		Assert.Equal(MatchStatus.Match, GetStatus(greenContent!, "TypingType"));
	}

	[Fact]
	public async Task Guess_TagsFeedback_CoversGreenYellowAndMiss()
	{
		var client = await GetAuthenticatedClientAsync("TechdleTagsStatusUser");
		var target = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["a", "b"]);
		var partialGuess = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["a", "c"]);
		var missGuess = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["x", "y"]);

		var puzzleId = await SeedPuzzleAsync(target, null, partialGuess, missGuess);

		var (_, partialContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = partialGuess.Name
		});

		var (_, missContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = missGuess.Name
		});

		var (_, greenContent, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = target.Name
		});

		Assert.NotNull(partialContent);
		Assert.NotNull(missContent);
		Assert.NotNull(greenContent);
		Assert.Equal(MatchStatus.Partial, GetStatus(partialContent!, "Tags"));
		Assert.Equal(MatchStatus.Miss, GetStatus(missContent!, "Tags"));
		Assert.Equal(MatchStatus.Match, GetStatus(greenContent!, "Tags"));
	}

	[Fact]
	public async Task Guess_TagsFeedback_WhenBothTagListsEmpty_ReturnsMatch()
	{
		var client = await GetAuthenticatedClientAsync("TechdleEmptyTagsUser");
		var target = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, []);
		var guess = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, []);

		var puzzleId = await SeedPuzzleAsync(target, null, guess);

		var (_, content, _) = await TechdlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<TechdleGuessResultDto>>(client, new TechdleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedLanguageName = guess.Name
		});

		Assert.NotNull(content);
		Assert.Equal(MatchStatus.Match, GetStatus(content!, "Tags"));
	}
}
