using Application.DTOs.TechnectionCategory;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class TechnectionCategorySuccesses
{
    public static Result<SuccessApiResponse<CreateTechnectionCategoryResponseDto>> CategoryCreated(CreateTechnectionCategoryResponseDto dto) =>
        Result<SuccessApiResponse<CreateTechnectionCategoryResponseDto>>.Success(new SuccessApiResponse<CreateTechnectionCategoryResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Technection category created successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetTechnectionCategoryByGroupNameResponseDto>> CategoryFetched(GetTechnectionCategoryByGroupNameResponseDto dto) =>
        Result<SuccessApiResponse<GetTechnectionCategoryByGroupNameResponseDto>>.Success(new SuccessApiResponse<GetTechnectionCategoryByGroupNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Technection category fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetPagedTechnectionCategoriesResponseDto>> CategoriesFetchedPaged(GetPagedTechnectionCategoriesResponseDto dto) =>
        Result<SuccessApiResponse<GetPagedTechnectionCategoriesResponseDto>>.Success(new SuccessApiResponse<GetPagedTechnectionCategoriesResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Technection categories fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<UpdateTechnectionCategoryByGroupNameResponseDto>> CategoryUpdated(UpdateTechnectionCategoryByGroupNameResponseDto dto) =>
        Result<SuccessApiResponse<UpdateTechnectionCategoryByGroupNameResponseDto>>.Success(new SuccessApiResponse<UpdateTechnectionCategoryByGroupNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Technection category updated successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<AddTechnectionCategoryWordByGroupNameResponseDto>> WordAdded(AddTechnectionCategoryWordByGroupNameResponseDto dto) =>
        Result<SuccessApiResponse<AddTechnectionCategoryWordByGroupNameResponseDto>>.Success(new SuccessApiResponse<AddTechnectionCategoryWordByGroupNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Word added to technection category successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<RemoveTechnectionCategoryWordByGroupNameResponseDto>> WordRemoved(RemoveTechnectionCategoryWordByGroupNameResponseDto dto) =>
        Result<SuccessApiResponse<RemoveTechnectionCategoryWordByGroupNameResponseDto>>.Success(new SuccessApiResponse<RemoveTechnectionCategoryWordByGroupNameResponseDto>
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
