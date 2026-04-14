using Application.Constants.Errors;
using Application.Constants.Successes;
using Application.DTOs.LogodlePlayer;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Models.DailyLogodle;
using Domain.Shared;

namespace Application.Services.Implementations;

public class LogodlePlayerService(
	ILogodleGameRepository logodleGameRepository,
	ILogodleTargetRepository logodleTargetRepository) : ILogodlePlayerService
{
	private const int MaxAttempts = 6;

	private readonly ILogodleGameRepository _logodleGameRepository = logodleGameRepository;
	private readonly ILogodleTargetRepository _logodleTargetRepository = logodleTargetRepository;

	public async Task<Result<SuccessApiResponse<LogodleGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken)
	{
		var puzzle = await _logodleGameRepository.GetByDateAsync(puzzleDate, cancellationToken);
		if (puzzle is null)
		{
			return Result<SuccessApiResponse<LogodleGameDto>>.Failure(LogodlePlayerErrors.GameNotFoundForDate);
		}

		var target = puzzle.Target;
		var initialImageUrl = GetBlurredImageForAttempt(target.BlurredImageUrls, 1, target.ImagePath);

		return LogodlePlayerSuccesses.GameFetched(new LogodleGameDto
		{
			PuzzleId = puzzle.Id,
			PuzzleDate = puzzle.PuzzleDate,
			InitialImageUrl = initialImageUrl
		});
	}

	public async Task<Result<SuccessApiResponse<LogodleGuessResultDto>>> EvaluateGuessAsync(LogodleGuessRequestDto request, CancellationToken cancellationToken)
	{
		var puzzle = await _logodleGameRepository.GetByIdAsync(request.PuzzleId, cancellationToken);
		if (puzzle is null)
		{
			return Result<SuccessApiResponse<LogodleGuessResultDto>>.Failure(LogodlePlayerErrors.InvalidPuzzleId);
		}

		var target = puzzle.Target;
		var isCorrect = string.Equals(request.GuessedTargetName?.Trim(), target.Name, StringComparison.OrdinalIgnoreCase);
		var isGameOver = isCorrect || request.AttemptNumber >= MaxAttempts;

		var revealedImageUrl = isGameOver
			? target.ImagePath
			: GetBlurredImageForAttempt(target.BlurredImageUrls, Math.Clamp(request.AttemptNumber, 1, MaxAttempts), target.ImagePath);

		return LogodlePlayerSuccesses.GuessEvaluated(new LogodleGuessResultDto
		{
			IsCorrect = isCorrect,
			IsGameOver = isGameOver,
			AttemptNumber = request.AttemptNumber,
			RevealedImageUrl = revealedImageUrl,
			TargetName = isGameOver ? target.Name : null
		});
	}

	public async Task<Result<SuccessApiResponse<CreateLogodleGamesResponseDto>>> CreateGamesAsync(CancellationToken cancellationToken)
	{
		var today = DateOnly.FromDateTime(DateTime.UtcNow);
		var latestPuzzleDate = await _logodleGameRepository.GetLatestPuzzleDateAsync(cancellationToken);
		var startPuzzleDate = latestPuzzleDate.HasValue && latestPuzzleDate.Value >= today
			? latestPuzzleDate.Value.AddDays(1)
			: today;

		var targets = await _logodleTargetRepository.GetAllAsync(cancellationToken);
		if (targets.Count == 0)
		{
			return Result<SuccessApiResponse<CreateLogodleGamesResponseDto>>.Failure(AdminLogodleErrors.NoTargetsFound);
		}

		Shuffle(targets);

		var puzzles = new List<DailyLogodle>(targets.Count);
		var responseItems = new List<CreateLogodleGameResponseDto>(targets.Count);

		for (var index = 0; index < targets.Count; index++)
		{
			var puzzleDate = startPuzzleDate.AddDays(index);
			var target = targets[index];
			var puzzle = new DailyLogodle(puzzleDate, target.Name);
			puzzles.Add(puzzle);

			responseItems.Add(new CreateLogodleGameResponseDto
			{
				PuzzleId = puzzle.Id,
				PuzzleDate = puzzleDate,
				TargetId = target.Id,
				TargetName = target.Name
			});
		}

		var created = await _logodleGameRepository.TryAddRangeAsync(puzzles, cancellationToken);
		if (!created)
		{
			return Result<SuccessApiResponse<CreateLogodleGamesResponseDto>>.Failure(AdminLogodleErrors.PuzzleAlreadyExists);
		}

		return AdminLogodleSuccesses.PuzzlesGenerated(new CreateLogodleGamesResponseDto
		{
			Items = responseItems
		});
	}

	private static void Shuffle<T>(IList<T> items)
	{
		for (var index = items.Count - 1; index > 0; index--)
		{
			var swapIndex = Random.Shared.Next(index + 1);
			(items[index], items[swapIndex]) = (items[swapIndex], items[index]);
		}
	}

	private static string GetBlurredImageForAttempt(IReadOnlyList<string> blurredImageUrls, int attemptNumber, string fallback)
	{
		if (blurredImageUrls.Count == 0)
		{
			return fallback;
		}

		// Stored list is least -> most blurred, while gameplay starts from most blurred.
		var clampedAttempt = Math.Clamp(attemptNumber, 1, blurredImageUrls.Count);
		var index = blurredImageUrls.Count - clampedAttempt;
		return blurredImageUrls[index];
	}
}