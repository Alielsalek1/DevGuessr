using Application.Constants.ApiErrors;
using Application.Constants.Successes;
using Application.DTOs.ExternalAuth;
using Application.Repositories.Interfaces;
using Application.Services.Implementations.Misc;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Enums;
using Domain.Extensions;
using Domain.Models.User;
using Domain.Models.UserRefreshTokens;
using Domain.Shared;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class ExternalAuthService(
    IUserRepository userRepo,
    IRefreshTokenProvider refreshTokenprovider,
    IConfiguration configuration,
    IGoogleAuthValidator googleAuthValidator,
    IUserRefreshTokensRepository userRefreshTokensRepository,
    JwtTokenProvider jwtTokenProvider,
    ILogger<ExternalAuthService> logger)
    : IExternalAuthService
{
    private readonly IUserRepository _userRepository = userRepo;
    private readonly IUserRefreshTokensRepository _userRefreshTokensRepository = userRefreshTokensRepository;
    private readonly IRefreshTokenProvider _refreshTokenProvider = refreshTokenprovider;
    private readonly JwtTokenProvider _jwtTokenProvider = jwtTokenProvider;
    private readonly IConfiguration _configuration = configuration;
    private readonly IGoogleAuthValidator _googleAuthValidator = googleAuthValidator;
    private readonly ILogger<ExternalAuthService> _logger = logger;

    public async Task<Result<SuccessApiResponse<GoogleAuthResponseDto>>> GoogleLoginAsync(GoogleAuthRequestDto authRequest, CancellationToken cancellationToken)
    {
        var payloadResult = await ValidateAndGetGooglePayloadAsync(authRequest.IdToken);
        if (!payloadResult.IsSuccess)
        {
            return Result<SuccessApiResponse<GoogleAuthResponseDto>>.Failure(payloadResult.Error);
        }
        var payload = payloadResult.Data;

        var user = await _userRepository.GetUserByGoogleIdAsync(payload.Subject, cancellationToken);
        // We have this part to allow internal login scheme users to be able to also login with google
        if (user == null) // not found by google id
        {
            user = await _userRepository.GetUserByEmailAsync(payload.Email, cancellationToken);
            if (user == null) // not found by email
            {
                user = await CreateExternalUserAsync(payload, cancellationToken);
            }
            // found by email but not linked to google, link the google account
            // the user is an internal auth scheme user, so we need to update the user to be eligible for external login
            else
            {
                user = await UpdateUserToBeEligibleForExternalLogin(user!, payload, cancellationToken);
            }
        }

        var refreshToken = _refreshTokenProvider.GenerateNewRefreshToken();
        await _userRefreshTokensRepository
            .AddUserRefreshTokenAsync(new UserRefreshToken(user!.Id, _refreshTokenProvider.HashRefreshToken(refreshToken)), cancellationToken);

        var accessToken = _jwtTokenProvider.GenerateAccessToken(user);
        _logger.LogInformation("Google login successful for user {UserId}", user.Id);

        return AuthSuccesses.GoogleAuthenticationSuccessful(new GoogleAuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id
        });
    }
    private async Task<Result<GoogleJsonWebSignature.Payload>> ValidateAndGetGooglePayloadAsync(string idToken)
    {
        var googleClientId = _configuration["Google:ClientId"];
        _logger.LogInformation("Validating Google token against ClientId: {ClientId}", googleClientId ?? "NULL");
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [googleClientId]
            };

            var payload = await _googleAuthValidator.ValidateAsync(idToken, settings);
            _logger.LogInformation("Google token validated for email: {Email}", payload.Email);
            return Result<GoogleJsonWebSignature.Payload>.Success(payload);
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Google token validation failed");
            return Result<GoogleJsonWebSignature.Payload>.Failure(AuthErrors.InvalidCredentials);
        }
    }
    private async Task<User> CreateExternalUserAsync(GoogleJsonWebSignature.Payload payload, CancellationToken ct)
    {
        var userCreationParams = new ExternalUserCreationParams
        {
            Email = payload.Email,
            GoogleId = payload.Subject,
            Role = Roles.User,
        };

        var user = new User(userCreationParams);
        user.MarkEmailAsVerified();

        await _userRepository.AddUserAsync(user, ct);
        return user;
    }

    public async Task<Result<SuccessApiResponse<GoogleAuthResponseDto>>> LinkGoogleAccountAsync(GoogleAuthRequestDto authRequest, Guid userId, CancellationToken cancellationToken)
    {
        var payloadResult = await ValidateAndGetGooglePayloadAsync(authRequest.IdToken);
        if (!payloadResult.IsSuccess)
        {
            return Result<SuccessApiResponse<GoogleAuthResponseDto>>.Failure(payloadResult.Error);
        }
        var payload = payloadResult.Data;

        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        var validationResult = await ValidateGoogleAccountLinkingAsync(payload, user, cancellationToken);
        if (!validationResult.IsSuccess)
        {
            return Result<SuccessApiResponse<GoogleAuthResponseDto>>.Failure(validationResult.Error);
        }

        await UpdateUserToBeEligibleForExternalLogin(user!, payload, cancellationToken);

        var refreshToken = _refreshTokenProvider.GenerateNewRefreshToken();
        await _userRefreshTokensRepository
            .AddUserRefreshTokenAsync(new UserRefreshToken(user!.Id, _refreshTokenProvider.HashRefreshToken(refreshToken)), cancellationToken);

        var accessToken = _jwtTokenProvider.GenerateAccessToken(user!);
        _logger.LogInformation("Google account linked successfully for user {UserId}", user!.Id);

        return AuthSuccesses.GoogleAuthenticationSuccessful(new GoogleAuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id
        });
    }
    private async Task<Result<User>> ValidateGoogleAccountLinkingAsync(GoogleJsonWebSignature.Payload payload, User? user, CancellationToken ct)
    {
        if (user == null)
        {
            _logger.LogWarning("Cannot link Google account: User not found");
            return Result<User>.Failure(UserErrors.UserNotFound);
        }
        if (user.IsNotGuest())
        {
            _logger.LogWarning("Cannot link Google account: User {UserId} is not a guest user", user!.Id);
            return Result<User>.Failure(UserErrors.UserIsNotGuest);
        }
        if (await IsEmailInUseAsync(payload.Email, ct))
        {
            _logger.LogWarning("Cannot link Google account: Email {Email} is already in use", payload.Email);
            return Result<User>.Failure(UserErrors.EmailAlreadyExists);
        }
        return Result<User>.Success(user!);
    }
    private async Task<bool> IsEmailInUseAsync(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.IsEmailInUseAsync(email, cancellationToken);
    }
    private async Task<User> UpdateUserToBeEligibleForExternalLogin(User user, GoogleJsonWebSignature.Payload payload, CancellationToken ct)
    {
        user.UpdateUserToBeEligibleForExternalLogin(payload.Email, payload.Subject);
        await _userRepository.UpdateUserAsync(user, ct);
        return user;
    }
}