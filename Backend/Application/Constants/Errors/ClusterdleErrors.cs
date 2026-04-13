using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class ClusterdleErrors
{
    public static readonly Error CategoryNotFound = new(
        ClusterdleErrorCodes.CategoryNotFound,
        "Technection category not found.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error CategoryGroupNameAlreadyExists = new(
        ClusterdleErrorCodes.CategoryGroupNameAlreadyExists,
        "A technection category with the given group name already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error WordNotFound = new(
        ClusterdleErrorCodes.WordNotFound,
        "The specified word was not found in this category.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error WordAlreadyExists = new(
        ClusterdleErrorCodes.WordAlreadyExists,
        "The specified word already exists in this category.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );
}
