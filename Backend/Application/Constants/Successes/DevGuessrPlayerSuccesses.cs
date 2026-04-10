using Application.DTOs.DevGuessrPlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class DevGuessrPlayerSuccesses
{
    public static Result<SuccessApiResponse<DevGuessrGameDto>> GameFetched(DevGuessrGameDto dto) =>
        Result<SuccessApiResponse<DevGuessrGameDto>>.Success(new SuccessApiResponse<DevGuessrGameDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Puzzle fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<CreateDevGuessrGameResponseDto>> PuzzleFetched(CreateDevGuessrGameResponseDto dto) =>
        Result<SuccessApiResponse<CreateDevGuessrGameResponseDto>>.Success(new SuccessApiResponse<CreateDevGuessrGameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Puzzle fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<DevGuessrGuessResultDto>> GuessEvaluated(DevGuessrGuessResultDto dto) =>
        Result<SuccessApiResponse<DevGuessrGuessResultDto>>.Success(new SuccessApiResponse<DevGuessrGuessResultDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Guess evaluated successfully.",
            Data = dto
        });
}
