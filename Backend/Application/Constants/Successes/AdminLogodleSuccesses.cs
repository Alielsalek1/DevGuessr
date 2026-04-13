using Application.DTOs.LogodlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class AdminLogodleSuccesses
{
    public static Result<SuccessApiResponse<CreateLogodleGamesResponseDto>> PuzzlesGenerated(CreateLogodleGamesResponseDto dto) =>
        Result<SuccessApiResponse<CreateLogodleGamesResponseDto>>.Success(new SuccessApiResponse<CreateLogodleGamesResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Logodle puzzles generated successfully.",
            Data = dto
        });
}
