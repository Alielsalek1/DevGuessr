using Domain.Constraints.MythdleTarget;
using Domain.Enums;

namespace Domain.Models.MythdleTarget;

public class MythdleTarget
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public bool IsFake { get; private set; }
    public string Description { get; private set; } = null!;
    public MythdleDifficulty Difficulty { get; private set; }

    // Parameterless constructor for EF Core
    private MythdleTarget() { }

    public MythdleTarget(MythdleTargetCreationParams creationParams)
    {
        MythdleTargetGuard.ValidateName(creationParams.Name);
        MythdleTargetGuard.ValidateCategory(creationParams.Category);
        MythdleTargetGuard.ValidateDescription(creationParams.Description);

        Name = creationParams.Name;
        Category = creationParams.Category;
        IsFake = creationParams.IsFake;
        Description = creationParams.Description;
        Difficulty = creationParams.Difficulty;
    }
}
