using Application.DTOs.LogodleTarget;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class LogodleTargetSuccesses
{
    public static Result<SuccessApiResponse<CreateLogodleTargetResponseDto>> TargetCreated(CreateLogodleTargetResponseDto dto) =>
        Result<SuccessApiResponse<CreateLogodleTargetResponseDto>>.Success(new SuccessApiResponse<CreateLogodleTargetResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Logodle target created successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetLogodleTargetByNameResponseDto>> TargetFetched(GetLogodleTargetByNameResponseDto dto) =>
        Result<SuccessApiResponse<GetLogodleTargetByNameResponseDto>>.Success(new SuccessApiResponse<GetLogodleTargetByNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Logodle target fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetPagedLogodleTargetsResponseDto>> TargetsFetchedPaged(GetPagedLogodleTargetsResponseDto dto) =>
        Result<SuccessApiResponse<GetPagedLogodleTargetsResponseDto>>.Success(new SuccessApiResponse<GetPagedLogodleTargetsResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Logodle targets fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse> TargetDeleted() =>
        Result<SuccessApiResponse>.Success(new SuccessApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Logodle target deleted successfully."
        });
}
