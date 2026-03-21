using Application.DTOs.User;
using Domain.Shared;
using Application.Utils;

namespace Application.Services.Interfaces;

public interface IUserFacadeService
{
    Task<Result<SuccessApiResponse>> UpdateProfileAsync(Guid userId, UpdateUserRequestDto request, CancellationToken cancellationToken);
    Task<Result<SuccessApiResponse<GetUserProfileResponseDto>>> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
}
