using Application.Constants.ApiErrors;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class AdminLangdleErrors
{
    public static readonly Error PuzzleAlreadyExists = new(
        AdminLangdleErrorCodes.PuzzleAlreadyExists,
        "A puzzle already exists for the selected date.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error NoLanguagesFound = new(
        AdminLangdleErrorCodes.NoLanguagesFound,
        "Cannot generate puzzle: No programming languages exist in the database.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );
}
