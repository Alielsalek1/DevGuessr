using Domain.Exceptions;

namespace Domain.Constraints.Clusterdle;

public static class ClusterdleGuard
{
    public static void NotNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{parameterName} cannot be null or empty.", parameterName);
    }

    public static void ValidateGroupName(string groupName)
    {
        NotNullOrEmpty(groupName, nameof(groupName));

        if (groupName.Length > 200)
            throw new DomainException("Group name cannot exceed 200 characters.", nameof(groupName));
    }

    public static void ValidateDifficultyLevel(int difficultyLevel)
    {
        if (difficultyLevel < 1 || difficultyLevel > 4)
            throw new DomainException("Difficulty level must be exactly 1, 2, 3, or 4.", nameof(difficultyLevel));
    }

    public static void ValidateWords(List<string> words)
    {
        if (words is null || words.Count == 0)
            throw new DomainException("Words list cannot be null or empty.", nameof(words));

        foreach (var word in words)
        {
            NotNullOrEmpty(word, "Word");

            if (word.Length > 200)
                throw new DomainException("Each word cannot exceed 200 characters.", "Word");
        }
    }

    public static void ValidateWord(string word)
    {
        NotNullOrEmpty(word, nameof(word));

        if (word.Length > 200)
            throw new DomainException("Word cannot exceed 200 characters.", nameof(word));
    }

    public static void ValidateSuccessMessage(string successMessage)
    {
        NotNullOrEmpty(successMessage, nameof(successMessage));

        if (successMessage.Length > 500)
            throw new DomainException("Success message cannot exceed 500 characters.", nameof(successMessage));
    }
}
