using Application.DTOs.TechnectionCategory;
using Application.Utils;
using Domain.Shared;

namespace Techdle.Application.Services.Interfaces.TechnectionCategory;

public interface ITechnectionCategoryService
{
    Task<Result<SuccessApiResponse<GetPagedTechnectionCategoriesResponseDto>>> GetPagedAsync(GetPagedTechnectionCategoriesRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<GetTechnectionCategoryByGroupNameResponseDto>>> GetByGroupNameAsync(string groupName, CancellationToken ct);
    Task<Result<SuccessApiResponse<CreateTechnectionCategoryResponseDto>>> CreateAsync(CreateTechnectionCategoryRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<UpdateTechnectionCategoryByGroupNameResponseDto>>> UpdateByGroupNameAsync(string groupName, UpdateTechnectionCategoryByGroupNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<AddTechnectionCategoryWordByGroupNameResponseDto>>> AddWordByGroupNameAsync(string groupName, AddTechnectionCategoryWordByGroupNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<RemoveTechnectionCategoryWordByGroupNameResponseDto>>> RemoveWordByGroupNameAsync(string groupName, RemoveTechnectionCategoryWordByGroupNameRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse>> DeleteByGroupNameAsync(string groupName, CancellationToken ct);
}
