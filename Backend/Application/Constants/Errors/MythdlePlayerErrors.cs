using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class MythdlePlayerErrors
{
    public static readonly Error PuzzleNotGenerated = new(
        MythdlePlayerErrorCodes.PuzzleNotGenerated,
        "Mythdle puzzle has not been generated yet.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );

    public static readonly Error InvalidPuzzleId = new(
        MythdlePlayerErrorCodes.InvalidPuzzleId,
        "Invalid or expired mythdle puzzle ID.",
        [],
        string.Empty,
        StatusCodes.Status400BadRequest
    );

    public static readonly Error GameNotFoundForDate = new(
        MythdlePlayerErrorCodes.GameNotFoundForDate,
        "Mythdle puzzle not found for the provided date.",
        [],
        string.Empty,
        StatusCodes.Status404NotFound
    );
}
