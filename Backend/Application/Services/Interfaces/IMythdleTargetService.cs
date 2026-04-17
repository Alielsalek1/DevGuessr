using Application.DTOs.MythdleTarget;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IMythdleTargetService
{
    Task<Result<SuccessApiResponse<GetPagedMythdleTargetsResponseDto>>> GetTargetsAsync(int pageNumber, int pageSize);
    Task<Result<SuccessApiResponse<CreateMythdleTargetResponseDto>>> CreateTargetAsync(CreateMythdleTargetDto request);
    Task<Result<SuccessApiResponse>> DeleteTargetByNameAsync(string name);
}
