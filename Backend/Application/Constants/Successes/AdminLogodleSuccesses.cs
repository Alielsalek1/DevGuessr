using Application.DTOs.LogodlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class AdminLogodleSuccesses
{
    public static Result<SuccessApiResponse<CreateLogodleGameResponseDto>> PuzzleGenerated(CreateLogodleGameResponseDto dto) =>
        Result<SuccessApiResponse<CreateLogodleGameResponseDto>>.Success(new SuccessApiResponse<CreateLogodleGameResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Logodle puzzle generated successfully.",
            Data = dto
        });
}
