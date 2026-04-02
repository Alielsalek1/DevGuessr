using Domain.Constraints.LogodleTarget;

namespace Domain.Models.LogodleTarget;

public class LogodleTarget
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public string ImagePath { get; private set; } = null!;
    public List<string> BlurredImageUrls { get; private set; } = [];

    // Parameterless constructor for EF Core
    private LogodleTarget() { }

    public LogodleTarget(LogodleTargetCreationParams creationParams)
    {
        LogodleTargetGuard.ValidateName(creationParams.Name);
        LogodleTargetGuard.ValidateImagePath(creationParams.ImagePath);
        LogodleTargetGuard.ValidateBlurredImageUrls(creationParams.BlurredImageUrls);

        Name = creationParams.Name;
        ImagePath = creationParams.ImagePath;
        BlurredImageUrls = creationParams.BlurredImageUrls;
    }

}
