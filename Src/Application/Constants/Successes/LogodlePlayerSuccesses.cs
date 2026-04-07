using Application.DTOs.LogodlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class LogodlePlayerSuccesses
{
    public static Result<SuccessApiResponse<LogodleGuessResultDto>> GuessEvaluated(LogodleGuessResultDto dto) =>
        Result<SuccessApiResponse<LogodleGuessResultDto>>.Success(new SuccessApiResponse<LogodleGuessResultDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Logodle guess evaluated successfully.",
            Data = dto
        });
}