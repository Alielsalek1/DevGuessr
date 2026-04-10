using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

		var userClient = new HttpClient(factory.Server.CreateHandler())
		{
			BaseAddress = factory.Server.BaseAddress
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

	private async Task SeedPuzzleAsync(DateOnly date, Guid targetId)
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var puzzle = new DailyLogodle(date, targetId);
		dbContext.DailyLogodles.Add(puzzle);
		await dbContext.SaveChangesAsync();
	}

	private async Task<DateOnly?> GetLatestPuzzleDateAsync()
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		return await dbContext.DailyLogodles
			.AsNoTracking()
			.OrderByDescending(d => d.PuzzleDate)
			.Select(d => (DateOnly?)d.PuzzleDate)
			.FirstOrDefaultAsync();
	}

	private async Task<DailyLogodle?> GetPuzzleAsync(Guid puzzleId)
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		return await dbContext.DailyLogodles
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == puzzleId);
	}

	[Fact]
	public async Task CreateGame_AdminWithTargets_Returns201CreatedWithPuzzleData()
	{
		var adminClient = await GetAdminClientAsync();
		var target = CreateTarget($"Target_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target);

		var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGameResponseDto>>();

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(content!.Success);
		Assert.Equal("Logodle puzzle generated successfully.", content.Message);
		Assert.NotEqual(Guid.Empty, content.Data.PuzzleId);
		Assert.Equal(target.Id, content.Data.TargetId);
		Assert.Equal(target.Name, content.Data.TargetName);
	}

	[Fact]
	public async Task CreateGame_NonAdminUser_Returns403Forbidden()
	{
		var userClient = await GetUserClientAsync();
		var target = CreateTarget($"Target_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target);

		var response = await userClient.PostAsync("/api/v1/logodle/games", null);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task CreateGame_AnonymousUser_Returns401Unauthorized()
	{
		var target = CreateTarget($"Target_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target);

		var response = await Client.PostAsync("/api/v1/logodle/games", null);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task CreateGame_EmptyTable_CreatesForUTCToday()
	{
		var adminClient = await GetAdminClientAsync();
		var target = CreateTarget($"Target_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target);

		var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
		var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGameResponseDto>>();

		var puzzle = await GetPuzzleAsync(content!.Data.PuzzleId);
		Assert.NotNull(puzzle);
		Assert.Equal(utcToday, puzzle.PuzzleDate);
	}

	[Fact]
	public async Task CreateGame_WithExistingPuzzle_CreatesAtMaxOfDatePlusOne()
	{
		var adminClient = await GetAdminClientAsync();
		var target = CreateTarget($"Target_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target);

		var existingDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-5);
		await SeedPuzzleAsync(existingDate, target.Id);

		var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGameResponseDto>>();

		var puzzle = await GetPuzzleAsync(content!.Data.PuzzleId);
		var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
		var expectedDate = utcToday > existingDate.AddDays(1) ? utcToday : existingDate.AddDays(1);
		Assert.Equal(expectedDate, puzzle.PuzzleDate);
	}

	[Fact]
	public async Task CreateGame_NoTargets_Returns400BadRequest()
	{
		var adminClient = await GetAdminClientAsync();

		var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
		var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Success);
		Assert.Equal("ERR_LOGODLE_NO_TARGETS_FOUND", content.ErrorCode);
	}

	[Fact]
	public async Task CreateGame_DuplicateDateConcurrentRequest_FirstSucceeds_SecondConflicts()
	{
		var adminClient1 = await GetAdminClientAsync();
		var adminClient2 = await GetAdminClientAsync();
		var target = CreateTarget($"Target_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target);

		var task1 = adminClient1.PostAsync("/api/v1/logodle/games", null);
		var task2 = adminClient2.PostAsync("/api/v1/logodle/games", null);

		var response1 = await task1;
		var response2 = await task2;

		var successResponse = response1.StatusCode == HttpStatusCode.Created ? response1 : response2;
		var conflictResponse = response1.StatusCode == HttpStatusCode.Conflict ? response1 : response2;

		if (response1.StatusCode == HttpStatusCode.Created && response2.StatusCode == HttpStatusCode.Created)
		{
			// Both could succeed if they used different dates; verify they're different
			var content1 = await response1.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGameResponseDto>>();
			var content2 = await response2.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGameResponseDto>>();
			var puzzle1 = await GetPuzzleAsync(content1!.Data.PuzzleId);
			var puzzle2 = await GetPuzzleAsync(content2!.Data.PuzzleId);
			Assert.NotEqual(puzzle1.PuzzleDate, puzzle2.PuzzleDate);
		}
		else
		{
			// One should succeed, one should conflict
			Assert.Equal(HttpStatusCode.Created, successResponse.StatusCode);
			Assert.Equal(HttpStatusCode.Conflict, conflictResponse.StatusCode);
		}
	}

	[Fact]
	public async Task CreateGame_PuzzleInsertedInDatabase()
	{
		var adminClient = await GetAdminClientAsync();
		var target = CreateTarget($"Target_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target);

		var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGameResponseDto>>();

		var puzzle = await GetPuzzleAsync(content!.Data.PuzzleId);
		Assert.NotNull(puzzle);
		Assert.Equal(content.Data.PuzzleId, puzzle.Id);
		Assert.Equal(content.Data.TargetId, puzzle.LogodleTargetId);
	}

	[Fact]
	public async Task CreateGame_RandomTargetSelected()
	{
		var adminClient = await GetAdminClientAsync();
		var target1 = CreateTarget($"Target1_{Guid.NewGuid():N}");
		var target2 = CreateTarget($"Target2_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target1, target2);

		var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateLogodleGameResponseDto>>();

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(
			content!.Data.TargetId == target1.Id || content.Data.TargetId == target2.Id,
			"Selected target must come from the seeded candidates.");
	}

	[Fact]
	public async Task CreateGame_PuzzleAlreadyExistsForDate_Returns409Conflict()
	{
		var adminClient = await GetAdminClientAsync();
		var target = CreateTarget($"Target_{Guid.NewGuid():N}");
		await SeedTargetsAsync(target);

		var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
		await SeedPuzzleAsync(utcToday, target.Id);

		var response = await adminClient.PostAsync("/api/v1/logodle/games", null);
		var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

		Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Success);
		Assert.Equal("ERR_LOGODLE_PUZZLE_ALREADY_EXISTS", content.ErrorCode);
	}
}
