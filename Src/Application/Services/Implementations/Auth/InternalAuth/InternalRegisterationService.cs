using Application.DTOs.Auth;
using Application.Services.Interfaces;
using Domain.Models.User;
using Domain.Shared;
using Domain.Enums;
using Application.Constants.ApiErrors;
using Application.Constants.Successes;
using Application.Utils;
using Application.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Application.Services.Interfaces.Auth.InternalAuth;
using Application.DTOs.Auth.InternalAuth;
using Application.Services.Implementations.Misc;
using Hangfire;

namespace Application.Services.Implementations;

public class InternalRegisterationService(
        IUserRepository userRepository,
        RegisterationConfirmationEmailSender registrationEmailSender,
        ILogger<InternalRegisterationService> logger,
        IOtpService<RegistrationOtpPayload> otpService,
        IBackgroundJobClient backgroundJobClient
    ) : IInternalRegisterationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<InternalRegisterationService> _logger = logger;
    private readonly RegisterationConfirmationEmailSender _emailService = registrationEmailSender;
    private readonly IOtpService<RegistrationOtpPayload> _otpService = otpService;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

    public async Task<Result<SuccessApiResponse<RegisterResponseDto>>> RegisterAsync(RegisterRequestDto registerRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating registration process for email {Email}", registerRequest.Email);
        var user = CreateUserForRegisteration(registerRequest);
        var registrationValidationResult = await ValidateRegisterRequestAsync(user, cancellationToken);
        if (!registrationValidationResult.IsSuccess)
        {
            return registrationValidationResult;
        }

        _logger.LogInformation("Creating user in database for email {Email}", registerRequest.Email);
        await _userRepository.AddUserAsync(user, cancellationToken);

        _logger.LogInformation("generating otp for email confirmation for user {UserId}", user.Id);
        string otp = OtpGenerator.GenerateOtp();
        await _otpService.CacheAsync(new RegistrationOtpPayload(user.Id), otp, cancellationToken);

        _logger.LogInformation("Sending confirmation email to {Email}", registerRequest.Email);
        _backgroundJobClient.Enqueue<RegisterationConfirmationEmailSender>(x => x.SendAsync(user.Email!, otp, CancellationToken.None));

        return AuthSuccesses.RegistrationSuccessful(new RegisterResponseDto
        {
            UserId = user.Id
        });
    }
    private static User CreateUserForRegisteration(RegisterRequestDto registerRequest)
    {
        var userCreationParams = new UserCreationParams
        {
            Email = registerRequest.Email,
            Username = registerRequest.Username,
            PhoneNumber = registerRequest.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password, BCrypt.Net.BCrypt.GenerateSalt()),
            Role = Roles.User
        };
        return new User(userCreationParams);
    }
    private async Task<Result<SuccessApiResponse<RegisterResponseDto>>> ValidateRegisterRequestAsync(User user, CancellationToken cancellationToken)
    {
        if (await IsEmailInUseAsync(user.Email!, cancellationToken))
        {
            _logger.LogWarning("Registration failed: Email {Email} already in use", user.Email);
            return Result<SuccessApiResponse<RegisterResponseDto>>.Failure(UserErrors.EmailAlreadyExists);
        }
        if (await IsUsernameInUseAsync(user.Username!, cancellationToken))
        {
            _logger.LogWarning("Registration failed: Username {Username} already in use", user.Username);
            return Result<SuccessApiResponse<RegisterResponseDto>>.Failure(UserErrors.UsernameAlreadyExists);
        }
        if (!string.IsNullOrEmpty(user.PhoneNumber) && await IsPhoneNumberInUseAsync(user.PhoneNumber, cancellationToken))
        {
            _logger.LogWarning("Registration failed: Phone number {PhoneNumber} already in use", user.PhoneNumber);
            return Result<SuccessApiResponse<RegisterResponseDto>>.Failure(UserErrors.PhoneNumberAlreadyExists);
        }
        return Result<SuccessApiResponse<RegisterResponseDto>>.Success(default!);
    }
    private async Task<bool> IsEmailInUseAsync(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.IsEmailInUseAsync(email, cancellationToken);
    }
    private async Task<bool> IsUsernameInUseAsync(string username, CancellationToken cancellationToken)
    {
        return await _userRepository.IsUsernameInUseAsync(username, cancellationToken);
    }
    private async Task<bool> IsPhoneNumberInUseAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        return await _userRepository.IsPhoneNumberInUseAsync(phoneNumber, cancellationToken);
    }

    public async Task<Result<SuccessApiResponse<RegisterResponseDto>>> GuestPromoteAsync(RegisterRequestDto registerRequest, Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating guest promotion process for user {UserId} with email {Email}", userId, registerRequest.Email);
        var user = CreateUserForRegisteration(registerRequest);
        user.SetGuestId(userId);
        var validateGuestPromototionRequestAsyncResult = await ValidateGuestPromoteRequestAsync(user, cancellationToken);
        if (!validateGuestPromototionRequestAsyncResult.IsSuccess)
        {
            return validateGuestPromototionRequestAsyncResult;
        }

        _logger.LogInformation("Updating user {UserId} in database for guest promotion", userId);
        await _userRepository.UpdateUserAsync(user, cancellationToken);

        _logger.LogInformation("Generating OTP for email confirmation for guest promotion for user {UserId}", userId);
        var otp = OtpGenerator.GenerateOtp();
        await _otpService.CacheAsync(new RegistrationOtpPayload(user.Id), otp, cancellationToken);

        _logger.LogInformation("Sending confirmation email for guest promotion to {Email}", registerRequest.Email);
        _backgroundJobClient.Enqueue<RegisterationConfirmationEmailSender>(x => x.SendAsync(user.Email!, otp, CancellationToken.None));

        return AuthSuccesses.RegistrationSuccessful(new RegisterResponseDto
        {
            UserId = user.Id
        });
    }
    private async Task<Result<SuccessApiResponse<RegisterResponseDto>>> ValidateGuestPromoteRequestAsync(User user, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(user.Id, cancellationToken);
        if (existingUser == null)
        {
            _logger.LogWarning("Guest promotion failed: User with ID {UserId} not found", user.Id);
            return Result<SuccessApiResponse<RegisterResponseDto>>.Failure(UserErrors.UserNotFound);
        }
        if (existingUser.IsGuest() == false)
        {
            _logger.LogWarning("Guest promotion failed: User with ID {UserId} is not a guest user", user.Id);
            return Result<SuccessApiResponse<RegisterResponseDto>>.Failure(UserErrors.UserIsNotGuest);
        }

        var RegisterRequestValidationResult = await ValidateRegisterRequestAsync(user, cancellationToken);
        if (!RegisterRequestValidationResult.IsSuccess)
        {
            return RegisterRequestValidationResult;
        }
        return Result<SuccessApiResponse<RegisterResponseDto>>.Success(default!);
    }
}