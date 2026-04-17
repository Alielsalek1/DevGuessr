using Application.DTOs.MythdlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class MythdlePlayerSuccesses
{
    public static Result<SuccessApiResponse<MythdleGameDto>> GameFetched(MythdleGameDto dto) =>
        Result<SuccessApiResponse<MythdleGameDto>>.Success(new SuccessApiResponse<MythdleGameDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Mythdle game fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<MythdleGuessResultDto>> GuessEvaluated(MythdleGuessResultDto dto) =>
        Result<SuccessApiResponse<MythdleGuessResultDto>>.Success(new SuccessApiResponse<MythdleGuessResultDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Mythdle guess evaluated successfully.",
            Data = dto
        });
}
