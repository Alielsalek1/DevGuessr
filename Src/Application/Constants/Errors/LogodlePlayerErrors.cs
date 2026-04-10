using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class LogodlePlayerErrors
{
	public static readonly Error PuzzleNotGenerated = new(
		LogodlePlayerErrorCodes.PuzzleNotGenerated,
		"Logodle puzzle has not been generated yet.",
		[],
		string.Empty,
		StatusCodes.Status404NotFound
	);

	public static readonly Error InvalidPuzzleId = new(
		LogodlePlayerErrorCodes.InvalidPuzzleId,
		"Invalid or expired logodle puzzle ID.",
		[],
		string.Empty,
		StatusCodes.Status400BadRequest
	);

	public static readonly Error GameNotFoundForDate = new(
		LogodlePlayerErrorCodes.GameNotFoundForDate,
		"Logodle puzzle not found for the provided date.",
		[],
		string.Empty,
		StatusCodes.Status404NotFound
	);
}