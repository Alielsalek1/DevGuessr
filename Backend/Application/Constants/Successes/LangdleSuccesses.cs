using Application.DTOs.Langdle;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class LangdleSuccesses
{
    public static Result<SuccessApiResponse<CreateLangdleResponseDto>> LanguageCreated(CreateLangdleResponseDto dto) =>
        Result<SuccessApiResponse<CreateLangdleResponseDto>>.Success(new SuccessApiResponse<CreateLangdleResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Programming language created successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetLangdleByNameResponseDto>> LanguageFetched(GetLangdleByNameResponseDto dto) =>
        Result<SuccessApiResponse<GetLangdleByNameResponseDto>>.Success(new SuccessApiResponse<GetLangdleByNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Programming language fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<AddLangdleTagByNameResponseDto>> TagAdded(AddLangdleTagByNameResponseDto dto) =>
        Result<SuccessApiResponse<AddLangdleTagByNameResponseDto>>.Success(new SuccessApiResponse<AddLangdleTagByNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Tag added to programming language successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<RemoveLangdleTagByNameResponseDto>> TagRemoved(RemoveLangdleTagByNameResponseDto dto) =>
        Result<SuccessApiResponse<RemoveLangdleTagByNameResponseDto>>.Success(new SuccessApiResponse<RemoveLangdleTagByNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Tag removed from programming language successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetPagedLangdleResponseDto>> LanguagesFetchedPaged(GetPagedLangdleResponseDto dto) =>
        Result<SuccessApiResponse<GetPagedLangdleResponseDto>>.Success(new SuccessApiResponse<GetPagedLangdleResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Programming languages fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<UpdateLangdleByNameResponseDto>> LanguageUpdated(UpdateLangdleByNameResponseDto dto) =>
        Result<SuccessApiResponse<UpdateLangdleByNameResponseDto>>.Success(new SuccessApiResponse<UpdateLangdleByNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Programming language updated successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse> LanguageDeleted() =>
        Result<SuccessApiResponse>.Success(new SuccessApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Programming language deleted successfully."
        });
}
