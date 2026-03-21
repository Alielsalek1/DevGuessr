using Application.Constants.ApiErrors;
using Application.Constants.Successes;
using Application.DTOs.User;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Models.User;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class UserService(IUserRepository userRepo, ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepo;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<Result<SuccessApiResponse>> UpdateProfileAsync(Guid userId, UpdateUserRequestDto request, CancellationToken ct)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, ct);
        var validationResult = await ValidateUpdateProfileRequestAsync(user, userId, request, ct);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }
        _logger.LogInformation("Updating profile for user {UserId}", userId);

        user!.UpdateAddress(request.Address);
        user.UpdatePhoneNumber(request.PhoneNumber);
        await _userRepository.UpdateUserAsync(user, ct);
        _logger.LogInformation("Profile updated successfully for user {UserId}", userId);

        return UserSuccesses.ProfileUpdated();
    }
    private async Task<Result<SuccessApiResponse>> ValidateUpdateProfileRequestAsync(
        User? user,
        Guid userId,
        UpdateUserRequestDto request,
        CancellationToken ct)
    {
        if (user is null)
        {
            _logger.LogWarning("Profile update failed: User {UserId} not found", userId);
            return Result<SuccessApiResponse>.Failure(UserErrors.UserNotFound);
        }

        if (request.PhoneNumber is not null && request.PhoneNumber != user.PhoneNumber)
        {
            _logger.LogDebug("Checking if phone number {PhoneNumber} is already in use", request.PhoneNumber);
            var isPhoneTaken = await _userRepository.IsPhoneNumberInUseAsync(request.PhoneNumber, ct);
            if (isPhoneTaken)
            {
                _logger.LogWarning("Profile update failed: Phone number {PhoneNumber} already in use", request.PhoneNumber);
                return Result<SuccessApiResponse>.Failure(UserErrors.PhoneNumberAlreadyExists);
            }
        }

        return Result<SuccessApiResponse>.Success(default!);
    }

    public async Task<Result<SuccessApiResponse<GetUserProfileResponseDto>>> GetProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, ct);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", userId);
            return Result<SuccessApiResponse<GetUserProfileResponseDto>>.Failure(UserErrors.UserNotFound);
        }
        _logger.LogInformation("Profile fetched successfully for user {UserId}", userId);

        return UserSuccesses.ProfileFetched(new GetUserProfileResponseDto
        {
            Id = user!.Id,
            Email = user!.Email!,
            Username = user!.Username!,
            PhoneNumber = user!.PhoneNumber ?? string.Empty,
            Address = user!.Address ?? string.Empty
        });
    }
}
