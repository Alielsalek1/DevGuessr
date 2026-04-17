using Application.DTOs.MythdleTarget;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class MythdleTargetSuccesses
{
    public static Result<SuccessApiResponse<CreateMythdleTargetResponseDto>> TargetCreated(CreateMythdleTargetResponseDto dto) =>
        Result<SuccessApiResponse<CreateMythdleTargetResponseDto>>.Success(new SuccessApiResponse<CreateMythdleTargetResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Mythdle target created successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetPagedMythdleTargetsResponseDto>> TargetsFetchedPaged(GetPagedMythdleTargetsResponseDto dto) =>
        Result<SuccessApiResponse<GetPagedMythdleTargetsResponseDto>>.Success(new SuccessApiResponse<GetPagedMythdleTargetsResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Mythdle targets fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse> TargetDeleted() =>
        Result<SuccessApiResponse>.Success(new SuccessApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Mythdle target deleted successfully."
        });
}
