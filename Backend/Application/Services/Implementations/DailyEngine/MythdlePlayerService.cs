using Application.Constants.Errors;
using Application.Constants.Successes;
using Application.DTOs.MythdlePlayer;
using Application.DTOs.MythdleTarget;
using Application.Repositories.Interfaces;
using Application.Services.Interfaces;
using Application.Utils;
using Domain.Models.DailyMythdle;
using Domain.Shared;
using MythdleTargetModel = Domain.Models.MythdleTarget.MythdleTarget;

namespace Application.Services.Implementations;

public class MythdlePlayerService(
    IMythdleGameRepository mythdleGameRepository,
    IMythdleTargetRepository mythdleTargetRepository) : IMythdlePlayerService
{
    private readonly IMythdleGameRepository _mythdleGameRepository = mythdleGameRepository;
    private readonly IMythdleTargetRepository _mythdleTargetRepository = mythdleTargetRepository;

    public async Task<Result<SuccessApiResponse<MythdleGameDto>>> GetGameByDateAsync(DateOnly puzzleDate, CancellationToken cancellationToken)
    {
        var puzzle = await _mythdleGameRepository.GetByDateAsync(puzzleDate, cancellationToken);
        if (puzzle is null)
        {
            return Result<SuccessApiResponse<MythdleGameDto>>.Failure(MythdlePlayerErrors.GameNotFoundForDate);
        }

        var targets = await GetPlayerTargetsByNamesAsync(puzzle.TargetNames, cancellationToken);

        return MythdlePlayerSuccesses.GameFetched(new MythdleGameDto
        {
            PuzzleId = puzzle.Id,
            PuzzleDate = puzzle.PuzzleDate,
            Targets = targets
        });
    }

    public async Task<Result<SuccessApiResponse<MythdleGuessResultDto>>> EvaluateGuessAsync(MythdleGuessRequestDto request, CancellationToken cancellationToken)
    {
        var puzzle = await _mythdleGameRepository.GetByIdAsync(request.PuzzleId, cancellationToken);
        if (puzzle is null)
        {
            return Result<SuccessApiResponse<MythdleGuessResultDto>>.Failure(MythdlePlayerErrors.InvalidPuzzleId);
        }

        var targetName = puzzle.Target.Name;
        var isCorrect = string.Equals(request.GuessedTargetName?.Trim(), targetName, StringComparison.OrdinalIgnoreCase);

        return MythdlePlayerSuccesses.GuessEvaluated(new MythdleGuessResultDto
        {
            IsCorrect = isCorrect,
            TargetName = isCorrect ? targetName : null,
            CorrectTargetName = targetName
        });
    }

    public async Task<Result<SuccessApiResponse<CreateMythdleGamesResponseDto>>> CreateGamesAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var latestPuzzleDate = await _mythdleGameRepository.GetLatestPuzzleDateAsync(cancellationToken);
        var startPuzzleDate = latestPuzzleDate.HasValue && latestPuzzleDate.Value >= today
            ? latestPuzzleDate.Value.AddDays(1)
            : today;

        var allTargets = await _mythdleTargetRepository.GetAllAsync(cancellationToken);
        var easyTargets = allTargets.Where(target => target.Difficulty == Domain.Enums.MythdleDifficulty.Easy && !target.IsFake).ToList();
        var mediumTargets = allTargets.Where(target => target.Difficulty == Domain.Enums.MythdleDifficulty.Medium && !target.IsFake).ToList();
        var hardTargets = allTargets.Where(target => target.Difficulty == Domain.Enums.MythdleDifficulty.Hard && !target.IsFake).ToList();
        var mythTargets = allTargets.Where(target => target.IsFake).ToList();

        // Per day: 2 easy + 2 medium + 1 hard + 1 fake = 6 cards
        if (easyTargets.Count < 2 || mediumTargets.Count < 2 || hardTargets.Count < 1 || mythTargets.Count < 1)
        {
            return Result<SuccessApiResponse<CreateMythdleGamesResponseDto>>.Failure(AdminMythdleErrors.NoTargetsFound);
        }

        Shuffle(easyTargets);
        Shuffle(mediumTargets);
        Shuffle(hardTargets);
        Shuffle(mythTargets);

        // Calculate potential games based on available pool
        var gameCount = new[] {
            easyTargets.Count / 2,
            mediumTargets.Count / 2,
            hardTargets.Count / 1,
            mythTargets.Count / 1,
        }.Min();

        if (gameCount == 0)
        {
            return Result<SuccessApiResponse<CreateMythdleGamesResponseDto>>.Failure(AdminMythdleErrors.NoTargetsFound);
        }

        var puzzles = new List<DailyMythdle>(gameCount);
        var responseItems = new List<CreateMythdleGameResponseDto>(gameCount);

        for (var index = 0; index < gameCount; index++)
        {
            var puzzleDate = startPuzzleDate.AddDays(index);
            var mythTarget = mythTargets[index];
            
            var selectedActuals = easyTargets.Skip(index * 2).Take(2)
                .Concat(mediumTargets.Skip(index * 2).Take(2))
                .Concat(hardTargets.Skip(index * 1).Take(1))
                .ToList();

            var selectedTargets = selectedActuals
                .Concat([mythTarget])
                .ToList();

            Shuffle(selectedTargets);

            var puzzle = new DailyMythdle(puzzleDate, mythTarget.Name, selectedTargets.Select(target => target.Name).ToList());
            puzzles.Add(puzzle);

            responseItems.Add(new CreateMythdleGameResponseDto
            {
                PuzzleId = puzzle.Id,
                PuzzleDate = puzzleDate,
                Targets = selectedTargets.Select(MapAdminTargetDto).ToList()
            });
        }

        var created = await _mythdleGameRepository.TryAddRangeAsync(puzzles, cancellationToken);
        if (!created)
        {
            return Result<SuccessApiResponse<CreateMythdleGamesResponseDto>>.Failure(AdminMythdleErrors.PuzzleAlreadyExists);
        }

        return AdminMythdleSuccesses.PuzzlesGenerated(new CreateMythdleGamesResponseDto
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

    private async Task<List<MythdlePlayerTargetDto>> GetPlayerTargetsByNamesAsync(IEnumerable<string> targetNames, CancellationToken cancellationToken)
    {
        var targetOrder = targetNames.ToList();
        var allTargets = await _mythdleTargetRepository.GetAllAsync(cancellationToken);
        var targetsByName = allTargets.ToDictionary(target => target.Name, StringComparer.OrdinalIgnoreCase);

        return targetOrder
            .Where(targetName => targetsByName.ContainsKey(targetName))
            .Select(targetName => MapPlayerTargetDto(targetsByName[targetName]))
            .ToList();
    }

    private static MythdlePlayerTargetDto MapPlayerTargetDto(MythdleTargetModel target) => new()
    {
        Name = target.Name,
        Category = target.Category,
        Description = target.Description
    };

    private static MythdleTargetDto MapAdminTargetDto(MythdleTargetModel target) => new()
    {
        Name = target.Name,
        Category = target.Category,
        IsFake = target.IsFake,
        Description = target.Description,
        Difficulty = target.Difficulty
    };
}
