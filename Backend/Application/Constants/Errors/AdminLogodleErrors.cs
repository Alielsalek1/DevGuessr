using Application.Constants.ApiErrors;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class AdminLogodleErrors
{
    public static readonly Error PuzzleAlreadyExists = new(
        AdminLogodleErrorCodes.PuzzleAlreadyExists,
        "A logodle puzzle already exists for the selected date.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error NoTargetsFound = new(
        AdminLogodleErrorCodes.NoTargetsFound,
        "Cannot generate puzzle: No logodle targets exist in the database.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );
}
