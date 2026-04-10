using Application.Constants.Errors;
using Application.Constants.Successes;
using Application.DTOs.DevGuessrPlayer;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Models.DailyDevGuessr;
using Domain.Enums;
using Domain.Shared;

namespace Application.Services.Implementations;

public class DevGuessrPlayerService(
    IDevGuessrGameRepository devGuessrGameRepository,
    IProgrammingLanguageRepository programmingLanguageRepository) : IDevGuessrPlayerService
{
    private readonly IDevGuessrGameRepository _devGuessrGameRepository = devGuessrGameRepository;
    private readonly IProgrammingLanguageRepository _programmingLanguageRepository = programmingLanguageRepository;

    public async Task<Result<SuccessApiResponse<DevGuessrGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken)
    {
        var puzzle = await _devGuessrGameRepository.GetByDateAsync(puzzleDate, cancellationToken);
        if (puzzle == null)
        {
            return Result<SuccessApiResponse<DevGuessrGameDto>>.Failure(DevGuessrPlayerErrors.GameNotFoundForDate);
        }

        return DevGuessrPlayerSuccesses.GameFetched(new DevGuessrGameDto
        {
            PuzzleId = puzzle.Id,
            PuzzleDate = puzzle.PuzzleDate
        });
    }

    public async Task<Result<SuccessApiResponse<DevGuessrGuessResultDto>>> EvaluateGuessAsync(DevGuessrGuessRequestDto request, CancellationToken cancellationToken)
    {
        var puzzle = await _devGuessrGameRepository.GetByIdAsync(request.PuzzleId, cancellationToken);
        if (puzzle == null)
        {
            return Result<SuccessApiResponse<DevGuessrGuessResultDto>>.Failure(DevGuessrPlayerErrors.InvalidPuzzleId);
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

    public async Task<Result<SuccessApiResponse<CreateDevGuessrGameResponseDto>>> CreateGameAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var latestPuzzleDate = await _devGuessrGameRepository.GetLatestPuzzleDateAsync(cancellationToken);
        var puzzleDate = latestPuzzleDate.HasValue && latestPuzzleDate.Value > today
            ? latestPuzzleDate.Value
            : today;

        var randomLanguage = await _programmingLanguageRepository.GetRandomAsync(cancellationToken);
        if (randomLanguage is null)
        {
            return Result<SuccessApiResponse<CreateDevGuessrGameResponseDto>>.Failure(AdminDevGuessrErrors.NoLanguagesFound);
        }

        var puzzle = new DailyDevGuessr(puzzleDate, randomLanguage.Id);
        var created = await _devGuessrGameRepository.TryAddAsync(puzzle, cancellationToken);
        if (!created)
        {
            return Result<SuccessApiResponse<CreateDevGuessrGameResponseDto>>.Failure(AdminDevGuessrErrors.PuzzleAlreadyExists);
        }

        return AdminDevGuessrSuccesses.PuzzleGenerated(new CreateDevGuessrGameResponseDto
        {
            PuzzleId = puzzle.Id,
            TargetId = randomLanguage.Id,
            TargetName = randomLanguage.Name
        });
    }

    private async Task<Result<SuccessApiResponse<DevGuessrGuessResultDto>>> EvaluateInternalAsync(
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
            return Result<SuccessApiResponse<DevGuessrGuessResultDto>>.Failure(DevGuessrPlayerErrors.GuessedLanguageNotFound);
        }

        var result = new DevGuessrGuessResultDto
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

        return DevGuessrPlayerSuccesses.GuessEvaluated(result);
    }
}
