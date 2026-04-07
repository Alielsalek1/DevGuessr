using System.Net;
using System.Net.Http.Headers;
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

namespace Tests.IntergrationTests.LogodlePlayer;

[Collection("Integration Tests")]
public class LogodlePlayerTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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

	private async Task<HttpClient> GetAuthenticatedClientAsync(string username)
	{
		var email = $"{username.ToLower()}@example.com";
		var (_, password, _, _) = await AuthBackdoor.CreateVerifiedUserAsync(username, email, "Pass123", Roles.User);

		var loginRequest = new LoginRequestDto
		{
			UsernameOrEmail = email,
			Password = password
		};

		var (_, loginContent, _) = await LoginTestHelpers.PostLoginAsync<SuccessApiResponse<LoginResponseDto>>(Client, loginRequest);
		Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
		return Client;
	}

	private async Task<(Guid PuzzleId, string TargetName, string ClearImage, string MostBlurred, string LeastBlurred)> SeedPuzzleAsync(string targetName)
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var target = CreateTarget(targetName);
		dbContext.LogodleTargets.Add(target);

		var puzzle = new DailyLogodle(DateOnly.FromDateTime(DateTime.UtcNow), target.Id);
		dbContext.DailyLogodles.Add(puzzle);

		await dbContext.SaveChangesAsync();

		return (puzzle.Id, target.Name, target.ImagePath, target.BlurredImageUrls[4], target.BlurredImageUrls[0]);
	}

	[Fact]
	public async Task Guess_WrongAttemptOne_ReturnsMostBlurredAndNotGameOver()
	{
		var client = await GetAuthenticatedClientAsync("LogodleAttemptOneUser");
		var (puzzleId, targetName, _, mostBlurred, _) = await SeedPuzzleAsync($"Target_{Guid.NewGuid():N}");

		var request = new LogodleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedTargetName = $"{targetName}_wrong",
			AttemptNumber = 1
		};

		var (response, content, json) = await LogodlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<LogodleGuessResultDto>>(client, request);

		Assert.True(response.StatusCode == HttpStatusCode.OK, $"Expected OK, got {response.StatusCode}. Output: {json}");
		Assert.NotNull(content);
		Assert.False(content!.Data.IsCorrect);
		Assert.False(content.Data.IsGameOver);
		Assert.Equal(mostBlurred, content.Data.RevealedImageUrl);
		Assert.Null(content.Data.TargetName);
	}

	[Fact]
	public async Task Guess_WrongAttemptFive_ReturnsLeastBlurredAndNotGameOver()
	{
		var client = await GetAuthenticatedClientAsync("LogodleAttemptFiveUser");
		var (puzzleId, targetName, _, _, leastBlurred) = await SeedPuzzleAsync($"Target_{Guid.NewGuid():N}");

		var request = new LogodleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedTargetName = $"{targetName}_wrong",
			AttemptNumber = 5
		};

		var (response, content, _) = await LogodlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<LogodleGuessResultDto>>(client, request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Data.IsCorrect);
		Assert.False(content.Data.IsGameOver);
		Assert.Equal(leastBlurred, content.Data.RevealedImageUrl);
		Assert.Null(content.Data.TargetName);
	}

	[Fact]
	public async Task Guess_WrongAttemptSix_RevealsClearImageAndEndsGame()
	{
		var client = await GetAuthenticatedClientAsync("LogodleAttemptSixUser");
		var (puzzleId, targetName, clearImage, _, _) = await SeedPuzzleAsync($"Target_{Guid.NewGuid():N}");

		var request = new LogodleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedTargetName = $"{targetName}_wrong",
			AttemptNumber = 6
		};

		var (response, content, _) = await LogodlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<LogodleGuessResultDto>>(client, request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Data.IsCorrect);
		Assert.True(content.Data.IsGameOver);
		Assert.Equal(clearImage, content.Data.RevealedImageUrl);
		Assert.Equal(targetName, content.Data.TargetName);
	}

	[Fact]
	public async Task Guess_WithCorrectName_RevealsClearImageAndEndsGame()
	{
		var client = await GetAuthenticatedClientAsync("LogodleCorrectGuessUser");
		var (puzzleId, targetName, clearImage, _, _) = await SeedPuzzleAsync($"Target_{Guid.NewGuid():N}");

		var request = new LogodleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedTargetName = targetName,
			AttemptNumber = 2
		};

		var (response, content, _) = await LogodlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<LogodleGuessResultDto>>(client, request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(content!.Data.IsCorrect);
		Assert.True(content.Data.IsGameOver);
		Assert.Equal(clearImage, content.Data.RevealedImageUrl);
		Assert.Equal(targetName, content.Data.TargetName);
	}

	[Fact]
	public async Task Guess_WithInvalidPuzzleId_Returns400BadRequest()
	{
		var client = await GetAuthenticatedClientAsync("LogodleInvalidPuzzleUser");
		var (_, targetName, _, _, _) = await SeedPuzzleAsync($"Target_{Guid.NewGuid():N}");

		var request = new LogodleGuessRequestDto
		{
			PuzzleId = Guid.NewGuid(),
			GuessedTargetName = targetName,
			AttemptNumber = 1
		};

		var (response, content, _) = await LogodlePlayerTestHelpers.PostGuessAsync<FailApiResponse>(client, request);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Success);
		Assert.Equal("ERR_LOGODLE_INVALID_PUZZLE_ID", content.ErrorCode);
	}

	[Fact]
	public async Task Guess_WhenPuzzleNotGenerated_Returns404NotFound()
	{
		var client = await GetAuthenticatedClientAsync("LogodleNoPuzzleUser");

		var request = new LogodleGuessRequestDto
		{
			PuzzleId = Guid.NewGuid(),
			GuessedTargetName = "anything",
			AttemptNumber = 1
		};

		var (response, content, _) = await LogodlePlayerTestHelpers.PostGuessAsync<FailApiResponse>(client, request);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Success);
		Assert.Equal("ERR_LOGODLE_PUZZLE_NOT_GENERATED", content.ErrorCode);
	}

	[Fact]
	public async Task Guess_WithoutAccessToken_AllowsAnonymousGuess()
	{
		var (puzzleId, targetName, clearImage, _, _) = await SeedPuzzleAsync($"Target_{Guid.NewGuid():N}");

		var request = new LogodleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedTargetName = targetName,
			AttemptNumber = 1
		};

		var (response, content, _) = await LogodlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<LogodleGuessResultDto>>(Client, request);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(content!.Data.IsCorrect);
		Assert.True(content.Data.IsGameOver);
		Assert.Equal(clearImage, content.Data.RevealedImageUrl);
	}

	[Fact]
	public async Task Guess_WhenTargetHasNoBlurredImages_UsesClearFallbackWhileNotGameOver()
	{
		var client = await GetAuthenticatedClientAsync("LogodleEmptyBlurredUser");
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var targetId = Guid.NewGuid();
		var puzzleId = Guid.NewGuid();
		var targetName = $"Target_{Guid.NewGuid():N}";
		var clearImage = $"/uploads/{targetName}/original.png";

		await dbContext.Database.ExecuteSqlRawAsync(
			"INSERT INTO logodle_targets (id, name, image_path, blurred_image_urls) VALUES ({0}, {1}, {2}, CAST({3} AS jsonb))",
			targetId,
			targetName,
			clearImage,
			"[]");

		await dbContext.Database.ExecuteSqlRawAsync(
			"INSERT INTO daily_logodles (id, puzzle_date, logodle_target_id) VALUES ({0}, {1}, {2})",
			puzzleId,
			DateOnly.FromDateTime(DateTime.UtcNow),
			targetId);

		var request = new LogodleGuessRequestDto
		{
			PuzzleId = puzzleId,
			GuessedTargetName = $"{targetName}_wrong",
			AttemptNumber = 1
		};

		var (response, content, _) = await LogodlePlayerTestHelpers.PostGuessAsync<SuccessApiResponse<LogodleGuessResultDto>>(client, request);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Data.IsCorrect);
		Assert.False(content.Data.IsGameOver);
		Assert.Equal(clearImage, content.Data.RevealedImageUrl);
		Assert.Null(content.Data.TargetName);
	}
}