using Application.DTOs.DevGuessrPlayer;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IDevGuessrPlayerService
{
    Task<Result<SuccessApiResponse<DevGuessrGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<DevGuessrGuessResultDto>>> EvaluateGuessAsync(DevGuessrGuessRequestDto request, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<CreateDevGuessrGameResponseDto>>> CreateGameAsync(CancellationToken cancellationToken);
}
