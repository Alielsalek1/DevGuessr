using Application.DTOs.TechdlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class TechdlePlayerSuccesses
{
    public static Result<SuccessApiResponse<TechdleLoadDto>> PuzzleFetched(TechdleLoadDto dto) =>
        Result<SuccessApiResponse<TechdleLoadDto>>.Success(new SuccessApiResponse<TechdleLoadDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Today's puzzle fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<TechdleGuessResultDto>> GuessEvaluated(TechdleGuessResultDto dto) =>
        Result<SuccessApiResponse<TechdleGuessResultDto>>.Success(new SuccessApiResponse<TechdleGuessResultDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Guess evaluated successfully.",
            Data = dto
        });
}
