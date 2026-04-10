using Application.DTOs.User;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class UserSuccesses
{
    public static Result<SuccessApiResponse> ProfileUpdated() =>
        Result<SuccessApiResponse>.Success(new SuccessApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Profile updated successfully."
        });

    public static Result<SuccessApiResponse<GetUserProfileResponseDto>> ProfileFetched(GetUserProfileResponseDto dto) =>
        Result<SuccessApiResponse<GetUserProfileResponseDto>>.Success(new SuccessApiResponse<GetUserProfileResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Profile fetched successfully.",
            Data = dto
        });
}
