using Application.DTOs.Auth;
using Application.DTOs.ExternalAuth;
using Application.DTOs.User;
using Application.Utils;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Successes;

public static class AuthSuccesses
{
    public static Result<SuccessApiResponse<LoginResponseDto>> LoginSuccessful(LoginResponseDto dto) =>
        Result<SuccessApiResponse<LoginResponseDto>>.Success(new SuccessApiResponse<LoginResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Login successful.",
            Data = dto
        });

    public static Result<SuccessApiResponse<LoginResponseDto>> NewDeviceDetected() =>
        Result<SuccessApiResponse<LoginResponseDto>>.Success(new SuccessApiResponse<LoginResponseDto>
        {
            StatusCode = StatusCodes.Status202Accepted,
            Message = "New device detected. A confirmation email has been sent to your email address. Please confirm to complete login."
        });

    public static Result<SuccessApiResponse<LoginResponseDto>> LoginConfirmed(LoginResponseDto dto) =>
        Result<SuccessApiResponse<LoginResponseDto>>.Success(new SuccessApiResponse<LoginResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Login confirmed successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<GuestLoginResponseDto>> GuestLoginSuccessful(GuestLoginResponseDto dto) =>
        Result<SuccessApiResponse<GuestLoginResponseDto>>.Success(new SuccessApiResponse<GuestLoginResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Guest login successful.",
            Data = dto
        });

    public static Result<SuccessApiResponse<RefreshTokenResponseDto>> TokenRefreshed(RefreshTokenResponseDto dto) =>
        Result<SuccessApiResponse<RefreshTokenResponseDto>>.Success(new SuccessApiResponse<RefreshTokenResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Access token refreshed successfully.",
            Data = dto
        });

    public static Result<SuccessApiResponse<RegisterResponseDto>> RegistrationSuccessful(RegisterResponseDto dto) =>
        Result<SuccessApiResponse<RegisterResponseDto>>.Success(new SuccessApiResponse<RegisterResponseDto>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = "User registered successfully. Please check your email for the confirmation code.",
            Data = dto
        });

    public static Result<SuccessApiResponse<ConfirmEmailResponseDto>> EmailConfirmed(ConfirmEmailResponseDto dto) =>
        Result<SuccessApiResponse<ConfirmEmailResponseDto>>.Success(new SuccessApiResponse<ConfirmEmailResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Email confirmation successful.",
            Data = dto
        });

    public static Result<SuccessApiResponse> ConfirmationEmailResent() =>
        Result<SuccessApiResponse>.Success(new SuccessApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Confirmation email resent successfully. Please check your email for the new confirmation code."
        });

    public static Result<SuccessApiResponse> PasswordResetEmailSent() =>
        Result<SuccessApiResponse>.Success(new SuccessApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Password reset email sent successfully. Please check your email for the reset code."
        });

    public static Result<SuccessApiResponse> PasswordResetSuccessful() =>
        Result<SuccessApiResponse>.Success(new SuccessApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Password reset successful."
        });

    public static Result<SuccessApiResponse<GoogleAuthResponseDto>> GoogleAuthenticationSuccessful(GoogleAuthResponseDto dto) =>
        Result<SuccessApiResponse<GoogleAuthResponseDto>>.Success(new SuccessApiResponse<GoogleAuthResponseDto>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Google authentication successful.",
            Data = dto
        });
}
