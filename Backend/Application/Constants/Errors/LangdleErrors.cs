using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class LangdleErrors
{
    public static readonly Error LanguageNotFound = new(
        LangdleErrorCodes.LanguageNotFound,
        "Programming language not found.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error LanguageNameAlreadyExists = new(
        LangdleErrorCodes.LanguageNameAlreadyExists,
        "A programming language with the given name already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static Error LanguageInvalidEnumValue(string fieldName, string value) => new(
        LangdleErrorCodes.LanguageInvalidEnumValue,
        $"Invalid value '{value}' for field '{fieldName}'.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );
    public static readonly Error TagAlreadyExists = new(
        LangdleErrorCodes.TagAlreadyExists,
        "The programming language already has this tag.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error TagNotFound = new(
        LangdleErrorCodes.TagNotFound,
        "The programming language does not have this tag.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );
}
