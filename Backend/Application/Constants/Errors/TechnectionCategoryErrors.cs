using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Techdle.Application.Constants.Errors;

public static class TechnectionCategoryErrors
{
    public static readonly Error CategoryNotFound = new(
        TechnectionCategoryErrorCodes.CategoryNotFound,
        "Technection category not found.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error CategoryGroupNameAlreadyExists = new(
        TechnectionCategoryErrorCodes.CategoryGroupNameAlreadyExists,
        "A technection category with the given group name already exists.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );

    public static readonly Error WordNotFound = new(
        TechnectionCategoryErrorCodes.WordNotFound,
        "The specified word was not found in this category.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error WordAlreadyExists = new(
        TechnectionCategoryErrorCodes.WordAlreadyExists,
        "The specified word already exists in this category.",
        [],
        string.Empty,
        StatusCodes.Status409Conflict
    );
}
