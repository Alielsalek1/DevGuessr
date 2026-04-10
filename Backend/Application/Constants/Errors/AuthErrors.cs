using Microsoft.AspNetCore.Http;

namespace Application.Constants.ApiErrors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = new(
        ErrorCodes.AuthErrorCodes.InvalidCredentials,
        "Invalid credentials.",
        [],
        string.Empty,
        StatusCodes.Status401Unauthorized
    );

    public static readonly Error EmailNotConfirmed = new(
        ErrorCodes.AuthErrorCodes.EmailNotConfirmed,
        "Email address has not been confirmed.",
        [],
        string.Empty,
        StatusCodes.Status403Forbidden
    );

    public static readonly Error EmailAlreadyConfirmed = new(
        ErrorCodes.AuthErrorCodes.EmailAlreadyConfirmed,
        "Email address is already confirmed.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );

    public static readonly Error InvalidToken = new(
        ErrorCodes.AuthErrorCodes.InvalidToken,
        "The provided token is invalid or has expired.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );

    public static readonly Error InvalidRefreshToken = new(
        ErrorCodes.AuthErrorCodes.InvalidRefreshToken,
        "The provided refresh token is invalid or has expired.",
        [],
        string.Empty,
        StatusCodes.Status401Unauthorized
    );

    public static readonly Error MissingRefreshToken = new(
        ErrorCodes.AuthErrorCodes.MissingRefreshToken,
        "Refresh token is missing or invalid.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );

    public static readonly Error WrongAuthScheme = new(
        ErrorCodes.AuthErrorCodes.WrongAuthScheme,
        "User is registered with a different authentication scheme.",
        [],
        string.Empty,
        StatusCodes.Status401Unauthorized
    );
}
