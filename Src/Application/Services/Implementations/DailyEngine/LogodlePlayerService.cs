using Application.Constants.Errors;
using Application.Constants.Successes;
using Application.DTOs.LogodlePlayer;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Implementations;

public class LogodlePlayerService(IDailyLogodleRepository dailyLogodleRepository) : ILogodlePlayerService
{
	private readonly IDailyLogodleRepository _dailyLogodleRepository = dailyLogodleRepository;

	public async Task<Result<SuccessApiResponse<LogodleGuessResultDto>>> EvaluateGuessAsync(LogodleGuessRequestDto request, CancellationToken cancellationToken)
	{
		var today = DateOnly.FromDateTime(DateTime.UtcNow);
		var puzzle = await _dailyLogodleRepository.GetByDateAsync(today, cancellationToken);

		if (puzzle is null)
		{
			return Result<SuccessApiResponse<LogodleGuessResultDto>>.Failure(LogodlePlayerErrors.PuzzleNotGenerated);
		}

		if (puzzle.Id != request.PuzzleId)
		{
			return Result<SuccessApiResponse<LogodleGuessResultDto>>.Failure(LogodlePlayerErrors.InvalidPuzzleId);
		}

		var target = puzzle.Target;
		var isCorrect = string.Equals(request.GuessedTargetName?.Trim(), target.Name, StringComparison.OrdinalIgnoreCase);
		var isGameOver = isCorrect || request.AttemptNumber >= 6;

		var revealedImageUrl = isGameOver
			? target.ImagePath
			: GetBlurredImageForAttempt(target.BlurredImageUrls, request.AttemptNumber, target.ImagePath);

		return LogodlePlayerSuccesses.GuessEvaluated(new LogodleGuessResultDto
		{
			IsCorrect = isCorrect,
			IsGameOver = isGameOver,
			AttemptNumber = request.AttemptNumber,
			RevealedImageUrl = revealedImageUrl,
			TargetName = isGameOver ? target.Name : null
		});
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