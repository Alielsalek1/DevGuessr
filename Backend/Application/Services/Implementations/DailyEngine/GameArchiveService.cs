using Application.Constants.Successes;
using Application.DTOs.Archive;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Shared;

namespace Application.Services.Implementations;

public class GameArchiveService(
    ILangdleGameRepository langdleGameRepository,
    ILogodleGameRepository logodleGameRepository,
    IMythdleGameRepository mythdleGameRepository) : IGameArchiveService
{
    private readonly ILangdleGameRepository _langdleGameRepository = langdleGameRepository;
    private readonly ILogodleGameRepository _logodleGameRepository = logodleGameRepository;
    private readonly IMythdleGameRepository _mythdleGameRepository = mythdleGameRepository;

    public async Task<Result<SuccessApiResponse<GetPastGamesResponseDto>>> GetPastGamesAsync(GetPastGamesRequestDto request, CancellationToken cancellationToken)
    {
        var skip = (request.PageNumber - 1) * request.PageSize;
        var take = request.PageSize;

        // Fetch past games from all three repositories sequentially to avoid
        // concurrently using the same EF Core DbContext instance (DbContext
        // is not thread-safe). Running queries in parallel caused
        // InvalidOperationException: "A second operation was started on this
        // context instance before a previous operation completed.".
        var (langdleItems, langdleTotalCount) = await _langdleGameRepository.GetPastPuzzlesAsync(skip, take, cancellationToken);
        var (logodleItems, logodleTotalCount) = await _logodleGameRepository.GetPastPuzzlesAsync(skip, take, cancellationToken);
        var (mythdleItems, mythdleTotalCount) = await _mythdleGameRepository.GetPastPuzzlesAsync(skip, take, cancellationToken);

        // Use the maximum total count (they should all be the same for the same date range)
        var totalCount = Math.Max(langdleTotalCount, Math.Max(logodleTotalCount, mythdleTotalCount));

        // Get all unique dates from all three game types
        var allDates = langdleItems.Select(d => d.PuzzleDate)
            .Union(logodleItems.Select(d => d.PuzzleDate))
            .Union(mythdleItems.Select(d => d.PuzzleDate))
            .OrderByDescending(d => d)
            .Distinct()
            .ToList();

        // Build daily game sets - grouping games by date
        var dailyGameSets = new List<DailyGameSetDto>();

        foreach (var date in allDates)
        {
            var langdleGame = langdleItems.FirstOrDefault(d => d.PuzzleDate == date);
            var logodleGame = logodleItems.FirstOrDefault(d => d.PuzzleDate == date);
            var mythdleGame = mythdleItems.FirstOrDefault(d => d.PuzzleDate == date);

            var dailySet = new DailyGameSetDto
            {
                PuzzleDate = date,
                Langdle = langdleGame != null
                    ? new PastLangdleGameDto
                    {
                        PuzzleId = langdleGame.Id,
                        PuzzleDate = langdleGame.PuzzleDate,
                        TargetName = langdleGame.TargetLanguage.Name
                    }
                    : null,
                Logodle = logodleGame != null
                    ? new PastLogodleGameDto
                    {
                        PuzzleId = logodleGame.Id,
                        PuzzleDate = logodleGame.PuzzleDate,
                        TargetName = logodleGame.Target.Name,
                        InitialImageUrl = logodleGame.Target.BlurredImageUrls.FirstOrDefault() ?? logodleGame.Target.ImagePath
                    }
                    : null,
                Mythdle = mythdleGame != null
                    ? new PastMythdleGameDto
                    {
                        PuzzleId = mythdleGame.Id,
                        PuzzleDate = mythdleGame.PuzzleDate,
                        TargetNames = mythdleGame.TargetNames
                    }
                    : null
            };

            dailyGameSets.Add(dailySet);
        }

        var response = new GetPastGamesResponseDto
        {
            Items = dailyGameSets,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return GameArchiveSuccesses.PastGamesFetched(response);
    }
}
