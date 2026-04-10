using Microsoft.AspNetCore.Http;

namespace Application.Constants.ApiErrors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new(
        UserErrorCodes.UserNotFound,
        "User not found.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error PhoneNumberAlreadyExists = new(
        UserErrorCodes.PhoneNumberAlreadyExists,
        "A user with the given phone number already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error UsernameAlreadyExists = new(
        UserErrorCodes.UsernameAlreadyExists,
        "A user with the given username already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error EmailAlreadyExists = new(
        UserErrorCodes.EmailAlreadyExists,
        "A user with the given email already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error UserIsNotGuest = new(
        UserErrorCodes.UserIsNotGuest,
        "The specified user is not a guest user.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );
}