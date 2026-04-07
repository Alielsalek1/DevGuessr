using Application.DTOs.TechdlePlayer;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface ITechdlePlayerService
{
    Task<Result<SuccessApiResponse<TechdleGuessResultDto>>> EvaluateGuessAsync(TechdleGuessRequestDto request, CancellationToken cancellationToken);
}
