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

	public async Task<Result<SuccessApiResponse<CreateLogodleGameResponseDto>>> CreateGameAsync(CancellationToken cancellationToken)
	{
		var today = DateOnly.FromDateTime(DateTime.UtcNow);
		var latestPuzzleDate = await _logodleGameRepository.GetLatestPuzzleDateAsync(cancellationToken);
		var puzzleDate = latestPuzzleDate.HasValue && latestPuzzleDate.Value > today
			? latestPuzzleDate.Value
			: today;

		var randomTarget = await _logodleTargetRepository.GetRandomAsync(cancellationToken);
		if (randomTarget is null)
		{
			return Result<SuccessApiResponse<CreateLogodleGameResponseDto>>.Failure(AdminLogodleErrors.NoTargetsFound);
		}

		var puzzle = new DailyLogodle(puzzleDate, randomTarget.Id);
		var created = await _logodleGameRepository.TryAddAsync(puzzle, cancellationToken);
		if (!created)
		{
			return Result<SuccessApiResponse<CreateLogodleGameResponseDto>>.Failure(AdminLogodleErrors.PuzzleAlreadyExists);
		}

		return AdminLogodleSuccesses.PuzzleGenerated(new CreateLogodleGameResponseDto
		{
			PuzzleId = puzzle.Id,
			TargetId = randomTarget.Id,
			TargetName = randomTarget.Name
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