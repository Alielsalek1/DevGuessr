using Application.Constants.ApiErrors;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class LangdlePlayerErrors
{
    public static readonly Error PuzzleNotGenerated = new(
        LangdlePlayerErrorCodes.PuzzleNotGenerated,
        "Puzzle has not been generated yet.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error InvalidPuzzleId = new(
        LangdlePlayerErrorCodes.InvalidPuzzleId,
        "Invalid or expired puzzle ID.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );

    public static readonly Error GameNotFoundForDate = new(
        LangdlePlayerErrorCodes.GameNotFoundForDate,
        "Puzzle not found for the provided date.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error GuessedLanguageNotFound = new(
        LangdlePlayerErrorCodes.GuessedLanguageNotFound,
        "Guessed programming language not found.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );
}
