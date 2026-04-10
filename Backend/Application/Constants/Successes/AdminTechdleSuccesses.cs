using Application.DTOs.TechdlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class AdminTechdleSuccesses
{
    public static Result<SuccessApiResponse<CreateTechdleGameResponseDto>> PuzzleGenerated(CreateTechdleGameResponseDto dto) =>
        Result<SuccessApiResponse<CreateTechdleGameResponseDto>>.Success(new SuccessApiResponse<CreateTechdleGameResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Techdle puzzle generated successfully.",
            Data = dto
        });
}
