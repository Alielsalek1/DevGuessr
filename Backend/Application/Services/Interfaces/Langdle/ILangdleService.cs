using Application.DTOs.Langdle;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces.Langdle;

public interface ILangdleService
{
    Task<Result<SuccessApiResponse<GetPagedLangdleResponseDto>>> GetPagedAsync(GetPagedLangdleRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<AddLangdleTagByNameResponseDto>>> AddTagByNameAsync(string name, AddLangdleTagByNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<RemoveLangdleTagByNameResponseDto>>> RemoveTagByNameAsync(string name, RemoveLangdleTagByNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<GetLangdleByNameResponseDto>>> GetByNameAsync(string name, CancellationToken ct);
    Task<Result<SuccessApiResponse<CreateLangdleResponseDto>>> CreateAsync(CreateLangdleRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<UpdateLangdleByNameResponseDto>>> UpdateByNameAsync(string name, UpdateLangdleByNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse>> DeleteByNameAsync(string name, CancellationToken ct);
}
