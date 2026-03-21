using Application.Constants.ApiErrors;
using Application.Constants.Successes;
using Application.DTOs.Auth;
using Application.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Application.Utils;
using Domain.Models.User;
using Application.Services.Implementations.Misc;
using Application.Services.Interfaces.Auth.InternalAuth;
using Domain.Extensions;
using Application.DTOs.Auth.InternalAuth;
using Domain.Shared;
using Application.Services.Interfaces;
using Hangfire;

namespace Application.Services.Implementations;

public class InternalPasswordResetService(
    IUserRepository userRepository,
    ILogger<InternalPasswordResetService> logger,
    PasswordResetEmailSender emailService,
    IOtpService<PasswordResetOtpPayload> otpService,
    IBackgroundJobClient backgroundJobClient
) : IInternalPasswordResetService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly PasswordResetEmailSender _emailService = emailService;
    private readonly ILogger<InternalPasswordResetService> _logger = logger;
    private readonly IOtpService<PasswordResetOtpPayload> _otpService = otpService;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

    public async Task<Result<SuccessApiResponse>> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating forget password process for email {Email}", forgetPasswordRequest.Email);
        var email = forgetPasswordRequest.Email;
        var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);
        var validationResult = await ValidateForgetPasswordRequestAsync(user, cancellationToken);
        if (validationResult.IsSuccess == false)
        {
            return validationResult;
        }

        _logger.LogInformation("Generating OTP for password reset for user {UserId}", user!.Id);
        var otp = OtpGenerator.GenerateOtp();
        await _otpService.CacheAsync(new PasswordResetOtpPayload(user!.Id), otp, cancellationToken);

        _logger.LogInformation("Sending password reset email to {Email}", email);
        _backgroundJobClient.Enqueue<PasswordResetEmailSender>(x => x.SendAsync(email, otp, CancellationToken.None));

        return AuthSuccesses.PasswordResetEmailSent();
    }
    private async Task<Result<SuccessApiResponse>> ValidateForgetPasswordRequestAsync(User? user, CancellationToken cancellationToken)
    {
        if (user == null)
        {
            _logger.LogWarning("Forget password failed: User not found");
            return Result<SuccessApiResponse>.Failure(UserErrors.UserNotFound);
        }
        if (user.IsNotEmailVerified())
        {
            _logger.LogWarning("Forget password failed: Email not confirmed");
            return Result<SuccessApiResponse>.Failure(AuthErrors.EmailNotConfirmed);
        }
        return Result<SuccessApiResponse>.Success(default!);
    }

    public async Task<Result<SuccessApiResponse>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating password reset process for OTP {Otp}", resetPasswordRequest.Otp);
        var userId = (await _otpService.GetDataAsync(resetPasswordRequest.Otp, cancellationToken))?.UserId ?? Guid.Empty;
        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Password reset failed: Invalid OTP");
            return Result<SuccessApiResponse>.Failure(AuthErrors.InvalidToken);
        }
        _logger.LogInformation("Fetching user details for user {UserId} ", userId);
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        var validationResult = await ValidateResetPasswordRequestAsync(user, cancellationToken);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        _logger.LogInformation("Resetting password for user {UserId} in Db", userId);
        var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(resetPasswordRequest.NewPassword, BCrypt.Net.BCrypt.GenerateSalt());
        await _userRepository.UpdatePasswordByEmailAsync(user!.Email!, newHashedPassword, cancellationToken);

        return AuthSuccesses.PasswordResetSuccessful();
    }
    private async Task<Result<SuccessApiResponse>> ValidateResetPasswordRequestAsync(User? user, CancellationToken cancellationToken)
    {
        if (user == null || user.IsNotEmailVerified())
        {
            _logger.LogWarning("Password reset failed: Invalid token for user {UserId}", user?.Id);
            return Result<SuccessApiResponse>.Failure(AuthErrors.InvalidToken);
        }
        return Result<SuccessApiResponse>.Success(default!);
    }
}