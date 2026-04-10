namespace Application.Constants.ApiErrors;
public static class UserErrorCodes
{
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string PhoneNumberAlreadyExists = "USER_PHONE_NUMBER_ALREADY_EXISTS";
    public const string UsernameAlreadyExists = "USER_USERNAME_ALREADY_EXISTS";
    public const string EmailAlreadyExists = "USER_EMAIL_ALREADY_EXISTS";
    public const string UserIsNotGuest = "USER_IS_NOT_GUEST";
}