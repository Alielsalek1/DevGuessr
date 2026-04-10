using Application.DTOs.User;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Interfaces;

public interface IUserService
{
    Task<Result<SuccessApiResponse>> UpdateProfileAsync(Guid userId, UpdateUserRequestDto request, CancellationToken ct);
    Task<Result<SuccessApiResponse<GetUserProfileResponseDto>>> GetProfileAsync(Guid userId, CancellationToken ct);
}
