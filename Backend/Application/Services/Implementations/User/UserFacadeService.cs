using Application.DTOs.User;
using Application.Services.Interfaces;
using Domain.Shared;
using Application.Utils;

namespace Application.Services.Implementations;

public class UserFacadeService(IUserService userService) : IUserFacadeService
{
    private readonly IUserService _userService = userService;

    public Task<Result<SuccessApiResponse>> UpdateProfileAsync(Guid userId, UpdateUserRequestDto request, CancellationToken cancellationToken)
        => _userService.UpdateProfileAsync(userId, request, cancellationToken);

    public Task<Result<SuccessApiResponse<GetUserProfileResponseDto>>> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
        => _userService.GetProfileAsync(userId, cancellationToken);
}
