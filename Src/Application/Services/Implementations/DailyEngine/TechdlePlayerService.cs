using Application.Constants.Errors;
using Application.Constants.Successes;
using Application.DTOs.TechdlePlayer;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Models.DailyTechdle;
using Domain.Enums;
using Domain.Shared;

namespace Application.Services.Implementations;

public class TechdlePlayerService(
    ITechdleGameRepository techdleGameRepository,
    IProgrammingLanguageRepository programmingLanguageRepository) : ITechdlePlayerService
{
    private readonly ITechdleGameRepository _techdleGameRepository = techdleGameRepository;
    private readonly IProgrammingLanguageRepository _programmingLanguageRepository = programmingLanguageRepository;

    public async Task<Result<SuccessApiResponse<TechdleGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken)
    {
        var puzzle = await _techdleGameRepository.GetByDateAsync(puzzleDate, cancellationToken);
        if (puzzle == null)
        {
            return Result<SuccessApiResponse<TechdleGameDto>>.Failure(TechdlePlayerErrors.GameNotFoundForDate);
        }

        return TechdlePlayerSuccesses.GameFetched(new TechdleGameDto
        {
            PuzzleId = puzzle.Id,
            PuzzleDate = puzzle.PuzzleDate
        });
    }

    public async Task<Result<SuccessApiResponse<TechdleGuessResultDto>>> EvaluateGuessAsync(TechdleGuessRequestDto request, CancellationToken cancellationToken)
    {
        var puzzle = await _techdleGameRepository.GetByIdAsync(request.PuzzleId, cancellationToken);
        if (puzzle == null)
        {
            return Result<SuccessApiResponse<TechdleGuessResultDto>>.Failure(TechdlePlayerErrors.InvalidPuzzleId);
        }

        var target = puzzle.TargetLanguage;
        return await EvaluateInternalAsync(request.GuessedLanguageName,
            target.Id, 
            target.YearFirstAppeared, 
            target.TypingDiscipline, 
            target.TypeStrength, 
            target.Tags, 
            cancellationToken);
    }

    public async Task<Result<SuccessApiResponse<CreateTechdleGameResponseDto>>> CreateGameAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var latestPuzzleDate = await _techdleGameRepository.GetLatestPuzzleDateAsync(cancellationToken);
        var puzzleDate = latestPuzzleDate.HasValue && latestPuzzleDate.Value > today
            ? latestPuzzleDate.Value
            : today;

        var randomLanguage = await _programmingLanguageRepository.GetRandomAsync(cancellationToken);
        if (randomLanguage is null)
        {
            return Result<SuccessApiResponse<CreateTechdleGameResponseDto>>.Failure(AdminTechdleErrors.NoLanguagesFound);
        }

        var puzzle = new DailyTechdle(puzzleDate, randomLanguage.Id);
        var created = await _techdleGameRepository.TryAddAsync(puzzle, cancellationToken);
        if (!created)
        {
            return Result<SuccessApiResponse<CreateTechdleGameResponseDto>>.Failure(AdminTechdleErrors.PuzzleAlreadyExists);
        }

        return AdminTechdleSuccesses.PuzzleGenerated(new CreateTechdleGameResponseDto
        {
            PuzzleId = puzzle.Id,
            TargetId = randomLanguage.Id,
            TargetName = randomLanguage.Name
        });
    }

    private async Task<Result<SuccessApiResponse<TechdleGuessResultDto>>> EvaluateInternalAsync(
        string guessedLanguageName,
        Guid targetId,
        int targetYear,
        TypingDiscipline targetTyping,
        TypeStrength targetStrength,
        List<string> targetTags,
        CancellationToken cancellationToken)
    {
        var guessed = await _programmingLanguageRepository.GetByNameAsync(guessedLanguageName, cancellationToken);
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
