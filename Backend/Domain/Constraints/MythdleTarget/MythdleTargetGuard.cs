using Domain.Exceptions;

namespace Domain.Constraints.MythdleTarget;

public static class MythdleTargetGuard
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
            throw new DomainException("Mythdle target name cannot exceed 100 characters.", nameof(name));
    }

    public static void ValidateCategory(string category)
    {
        NotNullOrEmpty(category, nameof(category));

        if (category.Length > 50)
            throw new DomainException("Mythdle target category cannot exceed 50 characters.", nameof(category));
    }

    public static void ValidateDescription(string description)
    {
        NotNullOrEmpty(description, nameof(description));

        if (description.Length > 500)
            throw new DomainException("Mythdle target description cannot exceed 500 characters.", nameof(description));
    }
}
