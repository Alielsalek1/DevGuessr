using Application.DTOs.TechdlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class TechdlePlayerSuccesses
{
    public static Result<SuccessApiResponse<TechdleGameDto>> GameFetched(TechdleGameDto dto) =>
        Result<SuccessApiResponse<TechdleGameDto>>.Success(new SuccessApiResponse<TechdleGameDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Puzzle fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<CreateTechdleGameResponseDto>> PuzzleFetched(CreateTechdleGameResponseDto dto) =>
        Result<SuccessApiResponse<CreateTechdleGameResponseDto>>.Success(new SuccessApiResponse<CreateTechdleGameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Puzzle fetched successfully.",
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
