using Application.Constants.Errors;
using Application.Constants.Successes;
using Application.DTOs.LangdlePlayer;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Models.DailyLangdle;
using Domain.Enums;
using Domain.Shared;

namespace Application.Services.Implementations;

public class LangdlePlayerService(
    ILangdleGameRepository langdleGameRepository,
    ILangdleRepository langdleRepository) : ILangdlePlayerService
{
    private readonly ILangdleGameRepository _langdleGameRepository = langdleGameRepository;
    private readonly ILangdleRepository _langdleRepository = langdleRepository;

    public async Task<Result<SuccessApiResponse<LangdleGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken)
    {
        var puzzle = await _langdleGameRepository.GetByDateAsync(puzzleDate, cancellationToken);
        if (puzzle == null)
        {
            return Result<SuccessApiResponse<LangdleGameDto>>.Failure(LangdlePlayerErrors.GameNotFoundForDate);
        }

        return LangdlePlayerSuccesses.GameFetched(new LangdleGameDto
        {
            PuzzleId = puzzle.Id,
            PuzzleDate = puzzle.PuzzleDate
        });
    }

    public async Task<Result<SuccessApiResponse<LangdleGuessResultDto>>> EvaluateGuessAsync(LangdleGuessRequestDto request, CancellationToken cancellationToken)
    {
        var puzzle = await _langdleGameRepository.GetByIdAsync(request.PuzzleId, cancellationToken);
        if (puzzle == null)
        {
            return Result<SuccessApiResponse<LangdleGuessResultDto>>.Failure(LangdlePlayerErrors.InvalidPuzzleId);
        }

        var target = puzzle.TargetLanguage;
        return await EvaluateInternalAsync(request.GuessedLanguageName,
            target.Id, 
            target.YearFirstAppeared, 
            target.TypingDiscipline, 
            target.TypeStrength, 
            target.ExecutionModel,
            target.MemoryManagement,
            target.Tags, 
            cancellationToken);
    }

    public async Task<Result<SuccessApiResponse<CreateLangdleGamesResponseDto>>> CreateGamesAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var latestPuzzleDate = await _langdleGameRepository.GetLatestPuzzleDateAsync(cancellationToken);
        var startPuzzleDate = latestPuzzleDate.HasValue && latestPuzzleDate.Value >= today
            ? latestPuzzleDate.Value.AddDays(1)
            : today;

        var languages = await _langdleRepository.GetAllAsync(cancellationToken);
        if (languages.Count == 0)
        {
            return Result<SuccessApiResponse<CreateLangdleGamesResponseDto>>.Failure(AdminLangdleErrors.NoLanguagesFound);
        }

        Shuffle(languages);

        var puzzles = new List<DailyLangdle>(languages.Count);
        var responseItems = new List<CreateLangdleGameResponseDto>(languages.Count);

        for (var index = 0; index < languages.Count; index++)
        {
            var puzzleDate = startPuzzleDate.AddDays(index);
            var language = languages[index];
            var puzzle = new DailyLangdle(puzzleDate, language.Name);
            puzzles.Add(puzzle);

            responseItems.Add(new CreateLangdleGameResponseDto
            {
                PuzzleId = puzzle.Id,
                PuzzleDate = puzzleDate,
                TargetId = language.Id,
                TargetName = language.Name
            });
        }

        var created = await _langdleGameRepository.TryAddRangeAsync(puzzles, cancellationToken);
        if (!created)
        {
            return Result<SuccessApiResponse<CreateLangdleGamesResponseDto>>.Failure(AdminLangdleErrors.PuzzleAlreadyExists);
        }

        return AdminLangdleSuccesses.PuzzlesGenerated(new CreateLangdleGamesResponseDto
        {
            Items = responseItems
        });
    }

    private static void Shuffle<T>(IList<T> items)
    {
        for (var index = items.Count - 1; index > 0; index--)
        {
            var swapIndex = Random.Shared.Next(index + 1);
            (items[index], items[swapIndex]) = (items[swapIndex], items[index]);
        }
    }

    private async Task<Result<SuccessApiResponse<LangdleGuessResultDto>>> EvaluateInternalAsync(
        string guessedLanguageName,
        Guid targetId,
        int targetYear,
        TypingDiscipline targetTyping,
        TypeStrength targetStrength,
        ExecutionModel targetExecutionModel,
        MemoryManagement targetMemoryManagement,
        List<string> targetTags,
        CancellationToken cancellationToken)
    {
        var guessed = await _langdleRepository.GetByNameAsync(guessedLanguageName, cancellationToken);
        if (guessed == null)
        {
            return Result<SuccessApiResponse<LangdleGuessResultDto>>.Failure(LangdlePlayerErrors.GuessedLanguageNotFound);
        }

        var result = new LangdleGuessResultDto
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

        // 2. TypingDiscipline
        result.AttributeFeedback.Add(new AttributeFeedback
        {
            AttributeName = "TypingDiscipline",
            GuessedValue = guessed.TypingDiscipline.ToString(),
            Status = targetTyping == guessed.TypingDiscipline ? MatchStatus.Match : MatchStatus.Miss
        });

        // 3. TypeStrength
        result.AttributeFeedback.Add(new AttributeFeedback
        {
            AttributeName = "TypeStrength",
            GuessedValue = guessed.TypeStrength.ToString(),
            Status = targetStrength == guessed.TypeStrength ? MatchStatus.Match : MatchStatus.Miss
        });

        // 4. ExecutionModel
        result.AttributeFeedback.Add(new AttributeFeedback
        {
            AttributeName = "ExecutionModel",
            GuessedValue = guessed.ExecutionModel.ToString(),
            Status = targetExecutionModel == guessed.ExecutionModel ? MatchStatus.Match : MatchStatus.Miss
        });

        // 5. MemoryManagement
        result.AttributeFeedback.Add(new AttributeFeedback
        {
            AttributeName = "MemoryManagement",
            GuessedValue = guessed.MemoryManagement.ToString(),
            Status = targetMemoryManagement == guessed.MemoryManagement ? MatchStatus.Match : MatchStatus.Miss
        });

        // 6. Tags
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

        return LangdlePlayerSuccesses.GuessEvaluated(result);
    }
}
