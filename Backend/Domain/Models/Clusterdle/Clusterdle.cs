using Domain.Constraints.Clusterdle;

namespace Domain.Models.Clusterdle;

public class Clusterdle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string GroupName { get; private set; } = null!;
    public int DifficultyLevel { get; private set; }
    public List<string> Words { get; private set; } = [];
    public string SuccessMessage { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private Clusterdle() { }

    public Clusterdle(ClusterdleCreationParams creationParams)
    {
        ClusterdleGuard.ValidateGroupName(creationParams.GroupName);
        ClusterdleGuard.ValidateDifficultyLevel(creationParams.DifficultyLevel);
        ClusterdleGuard.ValidateWords(creationParams.Words);
        ClusterdleGuard.ValidateSuccessMessage(creationParams.SuccessMessage);

        GroupName = creationParams.GroupName;
        DifficultyLevel = creationParams.DifficultyLevel;
        Words = creationParams.Words;
        SuccessMessage = creationParams.SuccessMessage;
    }

    public void Update(
        string? groupName = null,
        int? difficultyLevel = null,
        string? successMessage = null)
    {
        if (groupName is not null)
        {
            ClusterdleGuard.ValidateGroupName(groupName);
            GroupName = groupName;
        }

        if (difficultyLevel is not null)
        {
            ClusterdleGuard.ValidateDifficultyLevel(difficultyLevel.Value);
            DifficultyLevel = difficultyLevel.Value;
        }

        if (successMessage is not null)
        {
            ClusterdleGuard.ValidateSuccessMessage(successMessage);
            SuccessMessage = successMessage;
        }
    }

    public void AddWord(string word)
    {
        ClusterdleGuard.ValidateWord(word);
        if (!Words.Contains(word, StringComparer.OrdinalIgnoreCase))
        {
            Words.Add(word);
        }
    }

    public void RemoveWord(string word)
    {
        ClusterdleGuard.ValidateWord(word);
        var existing = Words.FirstOrDefault(w => w.Equals(word, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            Words.Remove(existing);
        }
    }
}
