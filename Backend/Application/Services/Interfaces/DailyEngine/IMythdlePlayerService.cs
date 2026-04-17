using Application.DTOs.MythdlePlayer;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IMythdlePlayerService
{
    Task<Result<SuccessApiResponse<MythdleGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<MythdleGuessResultDto>>> EvaluateGuessAsync(MythdleGuessRequestDto request, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<CreateMythdleGamesResponseDto>>> CreateGamesAsync(CancellationToken cancellationToken);
}
