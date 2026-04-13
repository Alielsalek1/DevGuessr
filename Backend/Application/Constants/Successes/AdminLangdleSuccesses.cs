using Application.DTOs.LangdlePlayer;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class AdminLangdleSuccesses
{
    public static Result<SuccessApiResponse<CreateLangdleGamesResponseDto>> PuzzlesGenerated(CreateLangdleGamesResponseDto dto) =>
        Result<SuccessApiResponse<CreateLangdleGamesResponseDto>>.Success(new SuccessApiResponse<CreateLangdleGamesResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Langdle puzzles generated successfully.",
            Data = dto
        });
}
