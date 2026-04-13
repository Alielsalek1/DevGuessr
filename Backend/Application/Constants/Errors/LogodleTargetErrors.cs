using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class LogodleTargetErrors
{
    public static readonly Error TargetNotFound = new(
        LogodleTargetErrorCodes.TargetNotFound,
        "Logodle target not found.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error TargetNameAlreadyExists = new(
        LogodleTargetErrorCodes.TargetNameAlreadyExists,
        "A logodle target with the given name already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );
}
