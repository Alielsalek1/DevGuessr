using Domain.Constraints.TechnectionCategory;

namespace Domain.Models.TechnectionCategory;

public class TechnectionCategory
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string GroupName { get; private set; } = null!;
    public int DifficultyLevel { get; private set; }
    public List<string> Words { get; private set; } = [];
    public string SuccessMessage { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private TechnectionCategory() { }

    public TechnectionCategory(TechnectionCategoryCreationParams creationParams)
    {
        TechnectionCategoryGuard.ValidateGroupName(creationParams.GroupName);
        TechnectionCategoryGuard.ValidateDifficultyLevel(creationParams.DifficultyLevel);
        TechnectionCategoryGuard.ValidateWords(creationParams.Words);
        TechnectionCategoryGuard.ValidateSuccessMessage(creationParams.SuccessMessage);

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
            TechnectionCategoryGuard.ValidateGroupName(groupName);
            GroupName = groupName;
        }

        if (difficultyLevel is not null)
        {
            TechnectionCategoryGuard.ValidateDifficultyLevel(difficultyLevel.Value);
            DifficultyLevel = difficultyLevel.Value;
        }

        if (successMessage is not null)
        {
            TechnectionCategoryGuard.ValidateSuccessMessage(successMessage);
            SuccessMessage = successMessage;
        }
    }

    public void AddWord(string word)
    {
        TechnectionCategoryGuard.ValidateWord(word);
        if (!Words.Contains(word, StringComparer.OrdinalIgnoreCase))
        {
            Words.Add(word);
        }
    }

    public void RemoveWord(string word)
    {
        TechnectionCategoryGuard.ValidateWord(word);
        var existing = Words.FirstOrDefault(w => w.Equals(word, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            Words.Remove(existing);
        }
    }
}
