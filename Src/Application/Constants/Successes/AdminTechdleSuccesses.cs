using Application.DTOs.TechdlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class AdminTechdleSuccesses
{
    public static Result<SuccessApiResponse<TechdleLoadDto>> PuzzleGenerated(TechdleLoadDto dto) =>
        Result<SuccessApiResponse<TechdleLoadDto>>.Success(new SuccessApiResponse<TechdleLoadDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Today's puzzle generated successfully.",
            Data = dto
        });
}
