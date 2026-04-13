using Application.DTOs.Clusterdle;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class ClusterdleSuccesses
{
    public static Result<SuccessApiResponse<CreateClusterdleResponseDto>> CategoryCreated(CreateClusterdleResponseDto dto) =>
        Result<SuccessApiResponse<CreateClusterdleResponseDto>>.Success(new SuccessApiResponse<CreateClusterdleResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Technection category created successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetClusterdleByGroupNameResponseDto>> CategoryFetched(GetClusterdleByGroupNameResponseDto dto) =>
        Result<SuccessApiResponse<GetClusterdleByGroupNameResponseDto>>.Success(new SuccessApiResponse<GetClusterdleByGroupNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Technection category fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetPagedClusterdleResponseDto>> CategoriesFetchedPaged(GetPagedClusterdleResponseDto dto) =>
        Result<SuccessApiResponse<GetPagedClusterdleResponseDto>>.Success(new SuccessApiResponse<GetPagedClusterdleResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Technection categories fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>> CategoryUpdated(UpdateClusterdleByGroupNameResponseDto dto) =>
        Result<SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>>.Success(new SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Technection category updated successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>> WordAdded(AddClusterdleWordByGroupNameResponseDto dto) =>
        Result<SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>>.Success(new SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Word added to technection category successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>> WordRemoved(RemoveClusterdleWordByGroupNameResponseDto dto) =>
        Result<SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>>.Success(new SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Word removed from technection category successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse> CategoryDeleted() =>
        Result<SuccessApiResponse>.Success(new SuccessApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Technection category deleted successfully."
        });
}
