using Domain.Exceptions;

namespace Domain.Constraints.ProgrammingLanguage;

public static class ProgrammingLanguageGuard
{
    public static void NotNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{parameterName} cannot be null or empty.", parameterName);
    }

    public static void ValidateName(string name)
    {
        NotNullOrEmpty(name, nameof(name));

        if (name.Length > 100)
            throw new DomainException("Programming language name cannot exceed 100 characters.", nameof(name));
    }

    public static void ValidateYearFirstAppeared(int year)
    {
        if (year < 1940 || year > DateTime.UtcNow.Year + 1)
            throw new DomainException($"Year first appeared must be between 1940 and {DateTime.UtcNow.Year + 1}.", nameof(year));
    }

    public static void ValidateTag(string tag)
    {
        NotNullOrEmpty(tag, nameof(tag));

        if (tag.Length > 50)
            throw new DomainException("Tag cannot exceed 50 characters.", nameof(tag));
    }
}
