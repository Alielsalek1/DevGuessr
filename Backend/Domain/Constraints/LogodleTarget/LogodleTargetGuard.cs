using Domain.Exceptions;

namespace Domain.Constraints.LogodleTarget;

public static class LogodleTargetGuard
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
            throw new DomainException("Logodle target name cannot exceed 100 characters.", nameof(name));
    }

    public static void ValidateImagePath(string imagePath)
    {
        NotNullOrEmpty(imagePath, nameof(imagePath));

        if (imagePath.Length > 500)
            throw new DomainException("Image path cannot exceed 500 characters.", nameof(imagePath));
    }

    public static void ValidateBlurredImageUrls(List<string> urls)
    {
        if (urls is null || urls.Count != 5)
            throw new DomainException("Exactly 5 blurred image URLs are required.", nameof(urls));

        foreach (var url in urls)
        {
            NotNullOrEmpty(url, "BlurredImageUrl");

            if (url.Length > 500)
                throw new DomainException("Blurred image URL cannot exceed 500 characters.", "BlurredImageUrl");
        }
    }
}
