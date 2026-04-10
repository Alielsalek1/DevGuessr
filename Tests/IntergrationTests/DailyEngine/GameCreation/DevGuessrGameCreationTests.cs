using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Application.DTOs.DevGuessrPlayer;
using Application.Utils;
using Domain.Enums;
using Domain.Models.DailyDevGuessr;
using Domain.Models.ProgrammingLanguage;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Auth;
using Tests.Common;
using TestsReusables.Auth;
using Xunit;

namespace Tests.IntergrationTests.DailyEngine.GameCreation;

[Collection("Integration Tests")]
public class DevGuessrGameCreationTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
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

		var userClient = new HttpClient(factory.Server.CreateHandler())
		{
			BaseAddress = factory.Server.BaseAddress
		};
		userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent!.Data.AccessToken);
		return userClient;
	}

	private async Task SeedLanguagesAsync(params ProgrammingLanguage[] languages)
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		dbContext.ProgrammingLanguages.AddRange(languages);
		await dbContext.SaveChangesAsync();
	}

	private async Task SeedPuzzleAsync(DateOnly date, Guid languageId)
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		var puzzle = new DailyDevGuessr(date, languageId);
		dbContext.DailyDevGuessrs.Add(puzzle);
		await dbContext.SaveChangesAsync();
	}

	private async Task<DateOnly?> GetLatestPuzzleDateAsync()
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		return await dbContext.DailyDevGuessrs
			.AsNoTracking()
			.OrderByDescending(d => d.PuzzleDate)
			.Select(d => (DateOnly?)d.PuzzleDate)
			.FirstOrDefaultAsync();
	}

	private async Task<DailyDevGuessr?> GetPuzzleAsync(Guid puzzleId)
	{
		using var scope = Factory.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		return await dbContext.DailyDevGuessrs
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Id == puzzleId);
	}

	[Fact]
	public async Task CreateGame_AdminWithLanguages_Returns201CreatedWithPuzzleData()
	{
		var adminClient = await GetAdminClientAsync();
		var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		await SeedLanguagesAsync(language);

		var response = await adminClient.PostAsync("/api/v1/devguessr/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateDevGuessrGameResponseDto>>();

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(content!.Success);
		Assert.Equal("DevGuessr puzzle generated successfully.", content.Message);
		Assert.NotEqual(Guid.Empty, content.Data.PuzzleId);
		Assert.Equal(language.Id, content.Data.TargetId);
		Assert.Equal(language.Name, content.Data.TargetName);
	}

	[Fact]
	public async Task CreateGame_NonAdminUser_Returns403Forbidden()
	{
		var userClient = await GetUserClientAsync();
		var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		await SeedLanguagesAsync(language);

		var response = await userClient.PostAsync("/api/v1/devguessr/games", null);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task CreateGame_AnonymousUser_Returns401Unauthorized()
	{
		var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		await SeedLanguagesAsync(language);

		var response = await Client.PostAsync("/api/v1/devguessr/games", null);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task CreateGame_EmptyTable_CreatesForUTCToday()
	{
		var adminClient = await GetAdminClientAsync();
		var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		await SeedLanguagesAsync(language);

		var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
		var response = await adminClient.PostAsync("/api/v1/devguessr/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateDevGuessrGameResponseDto>>();

		var puzzle = await GetPuzzleAsync(content!.Data.PuzzleId);
		Assert.NotNull(puzzle);
		Assert.Equal(utcToday, puzzle.PuzzleDate);
	}

	[Fact]
	public async Task CreateGame_WithExistingPuzzle_CreatesAtMaxOfDatePlusOne()
	{
		var adminClient = await GetAdminClientAsync();
		var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		await SeedLanguagesAsync(language);

		var existingDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-5);
		await SeedPuzzleAsync(existingDate, language.Id);

		var response = await adminClient.PostAsync("/api/v1/devguessr/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateDevGuessrGameResponseDto>>();

		var puzzle = await GetPuzzleAsync(content!.Data.PuzzleId);
		var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
		var expectedDate = utcToday > existingDate.AddDays(1) ? utcToday : existingDate.AddDays(1);
		Assert.Equal(expectedDate, puzzle.PuzzleDate);
	}

	[Fact]
	public async Task CreateGame_NoLanguages_Returns400BadRequest()
	{
		var adminClient = await GetAdminClientAsync();

		var response = await adminClient.PostAsync("/api/v1/devguessr/games", null);
		var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Success);
		Assert.Equal("ERR_NO_LANGUAGES_FOUND", content.ErrorCode);
	}

	[Fact]
	public async Task CreateGame_DuplicateDateConcurrentRequest_FirstSucceeds_SecondConflicts()
	{
		var adminClient1 = await GetAdminClientAsync();
		var adminClient2 = await GetAdminClientAsync();
		var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		await SeedLanguagesAsync(language);

		var task1 = adminClient1.PostAsync("/api/v1/devguessr/games", null);
		var task2 = adminClient2.PostAsync("/api/v1/devguessr/games", null);

		var response1 = await task1;
		var response2 = await task2;

		var successResponse = response1.StatusCode == HttpStatusCode.Created ? response1 : response2;
		var conflictResponse = response1.StatusCode == HttpStatusCode.Conflict ? response1 : response2;

		if (response1.StatusCode == HttpStatusCode.Created && response2.StatusCode == HttpStatusCode.Created)
		{
			// Both could succeed if they used different dates; verify they're different
			var content1 = await response1.Content.ReadFromJsonAsync<SuccessApiResponse<CreateDevGuessrGameResponseDto>>();
			var content2 = await response2.Content.ReadFromJsonAsync<SuccessApiResponse<CreateDevGuessrGameResponseDto>>();
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
		var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		await SeedLanguagesAsync(language);

		var response = await adminClient.PostAsync("/api/v1/devguessr/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateDevGuessrGameResponseDto>>();

		var puzzle = await GetPuzzleAsync(content!.Data.PuzzleId);
		Assert.NotNull(puzzle);
		Assert.Equal(content.Data.PuzzleId, puzzle.Id);
		Assert.Equal(content.Data.TargetId, puzzle.ProgrammingLanguageId);
	}

	[Fact]
	public async Task CreateGame_RandomLanguageSelected()
	{
		var adminClient = await GetAdminClientAsync();
		var lang1 = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		var lang2 = CreateLanguage(1995, TypingDiscipline.Dynamic, TypeStrength.Weak, ["scripting"]);
		await SeedLanguagesAsync(lang1, lang2);

		var response = await adminClient.PostAsync("/api/v1/devguessr/games", null);
		var content = await response.Content.ReadFromJsonAsync<SuccessApiResponse<CreateDevGuessrGameResponseDto>>();

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		Assert.NotNull(content);
		Assert.True(
			content!.Data.TargetId == lang1.Id || content.Data.TargetId == lang2.Id,
			"Selected language must come from the seeded candidates.");
	}

	[Fact]
	public async Task CreateGame_PuzzleAlreadyExistsForDate_Returns409Conflict()
	{
		var adminClient = await GetAdminClientAsync();
		var language = CreateLanguage(2000, TypingDiscipline.Static, TypeStrength.Strong, ["systems"]);
		await SeedLanguagesAsync(language);

		var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
		await SeedPuzzleAsync(utcToday, language.Id);

		var response = await adminClient.PostAsync("/api/v1/devguessr/games", null);
		var content = await response.Content.ReadFromJsonAsync<FailApiResponse>();

		Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
		Assert.NotNull(content);
		Assert.False(content!.Success);
		Assert.Equal("ERR_PUZZLE_ALREADY_EXISTS", content.ErrorCode);
	}
}
