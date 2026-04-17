using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class AdminMythdleErrors
{
    public static readonly Error PuzzleAlreadyExists = new(
        AdminMythdleErrorCodes.PuzzleAlreadyExists,
        "A mythdle puzzle already exists for the selected date.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error NoTargetsFound = new(
        AdminMythdleErrorCodes.NoTargetsFound,
        "Cannot generate puzzle: No mythdle targets exist in the database.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );
}
