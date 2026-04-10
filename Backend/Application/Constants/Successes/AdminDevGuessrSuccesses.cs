using Application.DTOs.DevGuessrPlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class AdminDevGuessrSuccesses
{
    public static Result<SuccessApiResponse<CreateDevGuessrGameResponseDto>> PuzzleGenerated(CreateDevGuessrGameResponseDto dto) =>
        Result<SuccessApiResponse<CreateDevGuessrGameResponseDto>>.Success(new SuccessApiResponse<CreateDevGuessrGameResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "DevGuessr puzzle generated successfully.",
            Data = dto
        });
}
