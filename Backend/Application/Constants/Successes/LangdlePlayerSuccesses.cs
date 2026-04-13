using Application.DTOs.LangdlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class LangdlePlayerSuccesses
{
    public static Result<SuccessApiResponse<LangdleGameDto>> GameFetched(LangdleGameDto dto) =>
        Result<SuccessApiResponse<LangdleGameDto>>.Success(new SuccessApiResponse<LangdleGameDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Puzzle fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<CreateLangdleGameResponseDto>> PuzzleFetched(CreateLangdleGameResponseDto dto) =>
        Result<SuccessApiResponse<CreateLangdleGameResponseDto>>.Success(new SuccessApiResponse<CreateLangdleGameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Puzzle fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<LangdleGuessResultDto>> GuessEvaluated(LangdleGuessResultDto dto) =>
        Result<SuccessApiResponse<LangdleGuessResultDto>>.Success(new SuccessApiResponse<LangdleGuessResultDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Guess evaluated successfully.",
            Data = dto
        });
}
