using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class MythdleTargetErrors
{
    public static readonly Error TargetNotFound = new(
        MythdleTargetErrorCodes.TargetNotFound,
        "Mythdle target not found.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error TargetNameAlreadyExists = new(
        MythdleTargetErrorCodes.TargetNameAlreadyExists,
        "A mythdle target with the given name already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );
}
