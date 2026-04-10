using Application.DTOs.LogodlePlayer;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface ILogodlePlayerService
{
	Task<Result<SuccessApiResponse<LogodleGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken);
	Task<Result<SuccessApiResponse<LogodleGuessResultDto>>> EvaluateGuessAsync(LogodleGuessRequestDto request, CancellationToken cancellationToken);
	Task<Result<SuccessApiResponse<CreateLogodleGameResponseDto>>> CreateGameAsync(CancellationToken cancellationToken);
}