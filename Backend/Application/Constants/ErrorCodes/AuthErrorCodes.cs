namespace Application.Constants.ErrorCodes;

public static class AuthErrorCodes
{
    public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
    public const string EmailNotConfirmed = "AUTH_EMAIL_NOT_CONFIRMED";
    public const string EmailAlreadyConfirmed = "AUTH_EMAIL_ALREADY_CONFIRMED";
    public const string InvalidToken = "AUTH_INVALID_TOKEN";
    public const string InvalidRefreshToken = "AUTH_INVALID_REFRESH_TOKEN";
    public const string MissingRefreshToken = "AUTH_MISSING_REFRESH_TOKEN";
    public const string WrongAuthScheme = "AUTH_WRONG_AUTH_SCHEME";
}