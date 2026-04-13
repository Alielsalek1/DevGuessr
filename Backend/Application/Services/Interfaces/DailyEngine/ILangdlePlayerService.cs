using Application.DTOs.LangdlePlayer;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface ILangdlePlayerService
{
    Task<Result<SuccessApiResponse<LangdleGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<LangdleGuessResultDto>>> EvaluateGuessAsync(LangdleGuessRequestDto request, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<CreateLangdleGamesResponseDto>>> CreateGamesAsync(CancellationToken cancellationToken);
}
