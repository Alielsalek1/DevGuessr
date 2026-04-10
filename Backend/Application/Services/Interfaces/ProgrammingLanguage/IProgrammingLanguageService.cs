using Application.DTOs.ProgrammingLanguage;
using Application.Utils;
using Domain.Shared;

namespace DevGuessr.Application.Services.Interfaces.ProgrammingLanguage;

public interface IProgrammingLanguageService
{
    Task<Result<SuccessApiResponse<GetPagedProgrammingLanguagesResponseDto>>> GetPagedAsync(GetPagedProgrammingLanguagesRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>>> AddTagByNameAsync(string name, AddProgrammingLanguageTagByNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>>> RemoveTagByNameAsync(string name, RemoveProgrammingLanguageTagByNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<GetProgrammingLanguageByNameResponseDto>>> GetByNameAsync(string name, CancellationToken ct);
    Task<Result<SuccessApiResponse<CreateProgrammingLanguageResponseDto>>> CreateAsync(CreateProgrammingLanguageRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<UpdateProgrammingLanguageByNameResponseDto>>> UpdateByNameAsync(string name, UpdateProgrammingLanguageByNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse>> DeleteByNameAsync(string name, CancellationToken ct);
}
