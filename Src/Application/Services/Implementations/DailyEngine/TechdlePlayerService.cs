using System.Text.Json;
using Application.Constants.Errors;
using Application.Constants.Successes;
using Application.DTOs.TechdlePlayer;
using Application.Models;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Enums;
using Domain.Shared;
using Domain.Models.DailyTechdle;
using Domain.Models.ProgrammingLanguage;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.Implementations;

public class TechdlePlayerService(
    IDistributedCache cache,
    IDailyTechdleRepository dailyTechdleRepository,
    IProgrammingLanguageRepository programmingLanguageRepository) : ITechdlePlayerService
{
    private readonly IDistributedCache _cache = cache;
    private readonly IDailyTechdleRepository _dailyTechdleRepository = dailyTechdleRepository;
    private readonly IProgrammingLanguageRepository _programmingLanguageRepository = programmingLanguageRepository;
 
    private static string GetCacheKey(DateOnly date) => $"Techdle_{date:yyyy-MM-dd}";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<Result<SuccessApiResponse<TechdleGuessResultDto>>> EvaluateGuessAsync(TechdleGuessRequestDto request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var cacheKey = GetCacheKey(today);
        
        // 1. Try Cache First
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedData))
        {
            var cacheModel = JsonSerializer.Deserialize<TechdlePuzzleCacheModel>(cachedData, JsonOptions);
            if (cacheModel != null && cacheModel.PuzzleId == request.PuzzleId)
            {
                return await EvaluateInternalAsync(request.GuessedLanguageId, 
                    cacheModel.TargetId, 
                    cacheModel.TargetYearFirstAppeared, 
                    cacheModel.TargetTypingDiscipline, 
                    cacheModel.TargetTypeStrength, 
                    cacheModel.TargetTags, 
                    cancellationToken);
            }
        }

        // 2. Fallback to Database
        var puzzle = await _dailyTechdleRepository.GetByDateAsync(today, cancellationToken);

        if (puzzle == null || puzzle.Id != request.PuzzleId)
        {
            return Result<SuccessApiResponse<TechdleGuessResultDto>>.Failure(TechdlePlayerErrors.InvalidPuzzleId);
        }

        var target = puzzle.TargetLanguage;
        return await EvaluateInternalAsync(request.GuessedLanguageId, 
            target.Id, 
            target.YearFirstAppeared, 
            target.TypingDiscipline, 
            target.TypeStrength, 
            target.Tags, 
            cancellationToken);
    }

    private async Task<Result<SuccessApiResponse<TechdleGuessResultDto>>> EvaluateInternalAsync(
        Guid guessedLanguageId,
        Guid targetId,
        int targetYear,
        TypingDiscipline targetTyping,
        TypeStrength targetStrength,
        List<string> targetTags,
        CancellationToken cancellationToken)
    {
        var guessed = await _programmingLanguageRepository.GetByIdAsync(guessedLanguageId, cancellationToken);
        if (guessed == null)
        {
            return Result<SuccessApiResponse<TechdleGuessResultDto>>.Failure(TechdlePlayerErrors.GuessedLanguageNotFound);
        }

        var result = new TechdleGuessResultDto
        {
            IsCorrect = targetId == guessed.Id,
            AttributeFeedback = new List<AttributeFeedback>()
        };

        // 1. ReleaseYear
        var yearStatus = targetYear == guessed.YearFirstAppeared ? MatchStatus.Match :
                         targetYear > guessed.YearFirstAppeared ? MatchStatus.Higher : MatchStatus.Lower;
        result.AttributeFeedback.Add(new AttributeFeedback
        {
            AttributeName = "ReleaseYear",
            GuessedValue = guessed.YearFirstAppeared.ToString(),
            Status = yearStatus
        });

        // 2. TypingType
        var typingStatus = MatchStatus.Miss;
        if (targetTyping == guessed.TypingDiscipline && targetStrength == guessed.TypeStrength)
        {
            typingStatus = MatchStatus.Match;
        }
        else if (targetTyping == guessed.TypingDiscipline || targetStrength == guessed.TypeStrength)
        {
            typingStatus = MatchStatus.Partial;
        }
        result.AttributeFeedback.Add(new AttributeFeedback
        {
            AttributeName = "TypingType",
            GuessedValue = $"{guessed.TypingDiscipline}, {guessed.TypeStrength}",
            Status = typingStatus
        });

        // 3. Tags
        var tagStatus = MatchStatus.Miss;
        var intersectedTags = targetTags.Intersect(guessed.Tags, StringComparer.OrdinalIgnoreCase).ToList();
        
        if (intersectedTags.Count == targetTags.Count && targetTags.Count == guessed.Tags.Count && targetTags.Count > 0)
        {
            tagStatus = MatchStatus.Match;
        }
        else if (intersectedTags.Count > 0)
        {
            tagStatus = MatchStatus.Partial;
        }
        else if (targetTags.Count == 0 && guessed.Tags.Count == 0)
        {
            tagStatus = MatchStatus.Match;
        }

        result.AttributeFeedback.Add(new AttributeFeedback
        {
            AttributeName = "Tags",
            GuessedValue = string.Join(", ", guessed.Tags),
            Status = tagStatus
        });

        return TechdlePlayerSuccesses.GuessEvaluated(result);
    }
}
