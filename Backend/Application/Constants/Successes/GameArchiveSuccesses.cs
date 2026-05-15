using Application.DTOs.Archive;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class GameArchiveSuccesses
{
    public static Result<SuccessApiResponse<GetPastGamesResponseDto>> PastGamesFetched(GetPastGamesResponseDto dto) =>
        Result<SuccessApiResponse<GetPastGamesResponseDto>>.Success(new SuccessApiResponse<GetPastGamesResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Past games fetched successfully.",
            Data = dto
        });
}
