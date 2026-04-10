using Application.Constants.ApiErrors;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class AdminDevGuessrErrors
{
    public static readonly Error PuzzleAlreadyExists = new(
        AdminDevGuessrErrorCodes.PuzzleAlreadyExists,
        "A puzzle already exists for the selected date.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error NoLanguagesFound = new(
        AdminDevGuessrErrorCodes.NoLanguagesFound,
        "Cannot generate puzzle: No programming languages exist in the database.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );
}
