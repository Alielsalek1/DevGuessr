using Application.DTOs.MythdlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class AdminMythdleSuccesses
{
    public static Result<SuccessApiResponse<CreateMythdleGamesResponseDto>> PuzzlesGenerated(CreateMythdleGamesResponseDto dto) =>
        Result<SuccessApiResponse<CreateMythdleGamesResponseDto>>.Success(new SuccessApiResponse<CreateMythdleGamesResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Mythdle puzzles generated successfully.",
            Data = dto
        });
}
