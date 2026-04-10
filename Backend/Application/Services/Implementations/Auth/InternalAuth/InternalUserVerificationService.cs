using Application.Constants.ApiErrors;
using Application.Constants.Successes;
using Application.DTOs.Auth;
using Application.DTOs.Auth.InternalAuth;
using Application.Repositories.Interfaces;
using Application.Services.Implementations.Misc;
using Application.Services.Interfaces;
using Application.Services.Interfaces.Auth.InternalAuth;
using Application.Utils;
using Domain.Models.User;
using Domain.Models.UserDevice;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using Hangfire;

namespace Application.Services.Implementations;

public class InternalUserVerificationService(
    IUserRepository userRepository,
    IOtpService<RegistrationOtpPayload> otpService,
    RegisterationConfirmationEmailSender emailService,
    ILogger<InternalUserVerificationService> logger,
    IUserDevicesRepository userDeviceRepository,
    IBackgroundJobClient backgroundJobClient
)
    : IInternalUserVerificationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IOtpService<RegistrationOtpPayload> _otpService = otpService;
    private readonly RegisterationConfirmationEmailSender _emailService = emailService;
    private readonly IUserDevicesRepository _userDeviceRepository = userDeviceRepository;
    private readonly ILogger<InternalUserVerificationService> _logger = logger;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

    public async Task<Result<SuccessApiResponse<ConfirmEmailResponseDto>>> ConfirmEmailAsync(ConfirmEmailRequestDto confirmEmailRequest, Guid deviceId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating email confirmation process for OTP {Otp} and device ID {DeviceId}", confirmEmailRequest.Otp, deviceId);
        var userId = (await _otpService.GetDataAsync(confirmEmailRequest.Otp, cancellationToken))?.UserId ?? Guid.Empty;
        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Email confirmation failed: Invalid OTP");
            return Result<SuccessApiResponse<ConfirmEmailResponseDto>>.Failure(AuthErrors.InvalidToken);
        }
        _logger.LogInformation("OTP valid, retrieved user ID {UserId} for email confirmation", userId);
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        var validationResult = await ValidateConfirmEmailRequestAsync(user);
        if (!validationResult.IsSuccess)
        {
            return Result<SuccessApiResponse<ConfirmEmailResponseDto>>.Failure(validationResult.Error);
        }
        _logger.LogInformation("Confirming email for user ID {UserId}", userId);
        await _userRepository.ConfirmEmailAsync(user!.Email!, cancellationToken);

        _logger.LogInformation("Email confirmed successfully for user ID {UserId}. Trusting device with ID {DeviceId}", userId, deviceId);
        await TrustDeviceAsync(userId, deviceId, cancellationToken);

        return AuthSuccesses.EmailConfirmed(new ConfirmEmailResponseDto { DeviceId = deviceId });
    }
    private async Task<Result<SuccessApiResponse>> ValidateConfirmEmailRequestAsync(User? user)
    {
        if (user == null)
        {
            _logger.LogWarning("Email confirmation failed: User not found or token expired");
            return Result<SuccessApiResponse>.Failure(AuthErrors.InvalidToken);
        }

        if (user!.IsEmailVerified)
        {
            _logger.LogWarning("Email confirmation failed: Email {Email} is already confirmed", user.Email);
            return Result<SuccessApiResponse>.Failure(AuthErrors.EmailAlreadyConfirmed);
        }
        return Result<SuccessApiResponse>.Success(default!);
    }
    private async Task TrustDeviceAsync(Guid userId, Guid deviceId, CancellationToken cancellationToken)
    {
        var UserDevice = new UserDevice(userId, deviceId);
        await _userDeviceRepository.AddUserDeviceAsync(UserDevice, cancellationToken);
    }

    public async Task<Result<SuccessApiResponse>> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto resendConfirmationEmailRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating resend confirmation email process for email {Email}", resendConfirmationEmailRequest.Email);
        var email = resendConfirmationEmailRequest.Email;

        _logger.LogInformation("Fetching user by email {Email} for resend confirmation email", email);
        var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);
        var validationResult = await ValidateResendConfirmationEmailRequestAsync(user, cancellationToken);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        _logger.LogInformation("Generating OTP for email confirmation for resend confirmation email for user ID {UserId}", user!.Id);
        var otp = OtpGenerator.GenerateOtp();
        await _otpService.CacheAsync(new RegistrationOtpPayload(user!.Id), otp, cancellationToken);

        _logger.LogInformation("Sending confirmation email for resend confirmation email to {Email}", email);
        _backgroundJobClient.Enqueue<RegisterationConfirmationEmailSender>(x => x.SendAsync(email, otp, CancellationToken.None));

        return AuthSuccesses.ConfirmationEmailResent();
    }
    private async Task<Result<SuccessApiResponse>> ValidateResendConfirmationEmailRequestAsync(User? user, CancellationToken cancellationToken)
    {
        if (user == null)
        {
            _logger.LogWarning("Resend confirmation failed: User not found");
            return Result<SuccessApiResponse>.Failure(UserErrors.UserNotFound);
        }
        if (user.IsEmailVerified)
        {
            _logger.LogWarning("Resend confirmation failed: Email {Email} is already confirmed", user.Email);
            return Result<SuccessApiResponse>.Failure(AuthErrors.EmailAlreadyConfirmed);
        }
        return Result<SuccessApiResponse>.Success(default!);
    }
}