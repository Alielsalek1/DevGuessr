using Application.Constants.ApiErrors;
using Microsoft.AspNetCore.Http;

namespace Application.Constants.Errors;

public static class LogodlePlayerErrors
{
	public static readonly Error PuzzleNotGenerated = new(
		LogodlePlayerErrorCodes.PuzzleNotGenerated,
		"Today's logodle puzzle has not been generated yet.",
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
}