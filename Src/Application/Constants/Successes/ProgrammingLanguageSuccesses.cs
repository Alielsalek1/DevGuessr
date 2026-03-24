using Application.DTOs.ProgrammingLanguage;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class ProgrammingLanguageSuccesses
{
    public static Result<SuccessApiResponse<CreateProgrammingLanguageResponseDto>> LanguageCreated(CreateProgrammingLanguageResponseDto dto) =>
        Result<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>.Success(new SuccessApiResponse<CreateProgrammingLanguageResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "Programming language created successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetProgrammingLanguageByNameResponseDto>> LanguageFetched(GetProgrammingLanguageByNameResponseDto dto) =>
        Result<SuccessApiResponse<GetProgrammingLanguageByNameResponseDto>>.Success(new SuccessApiResponse<GetProgrammingLanguageByNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Programming language fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>> TagAdded(AddProgrammingLanguageTagByNameResponseDto dto) =>
        Result<SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>>.Success(new SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Tag added to programming language successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>> TagRemoved(RemoveProgrammingLanguageTagByNameResponseDto dto) =>
        Result<SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>>.Success(new SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Tag removed from programming language successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GetPagedProgrammingLanguagesResponseDto>> LanguagesFetchedPaged(GetPagedProgrammingLanguagesResponseDto dto) =>
        Result<SuccessApiResponse<GetPagedProgrammingLanguagesResponseDto>>.Success(new SuccessApiResponse<GetPagedProgrammingLanguagesResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Programming languages fetched successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<UpdateProgrammingLanguageByNameResponseDto>> LanguageUpdated(UpdateProgrammingLanguageByNameResponseDto dto) =>
        Result<SuccessApiResponse<UpdateProgrammingLanguageByNameResponseDto>>.Success(new SuccessApiResponse<UpdateProgrammingLanguageByNameResponseDto>
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
