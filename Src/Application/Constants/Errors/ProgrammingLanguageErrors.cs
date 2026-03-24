using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Techdle.Application.Constants.Errors;

public static class ProgrammingLanguageErrors
{
    public static readonly Error LanguageNotFound = new(
        ProgrammingLanguageErrorCodes.LanguageNotFound,
        "Programming language not found.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error LanguageNameAlreadyExists = new(
        ProgrammingLanguageErrorCodes.LanguageNameAlreadyExists,
        "A programming language with the given name already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static Error LanguageInvalidEnumValue(string fieldName, string value) => new(
        ProgrammingLanguageErrorCodes.LanguageInvalidEnumValue,
        $"Invalid value '{value}' for field '{fieldName}'.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );
}
