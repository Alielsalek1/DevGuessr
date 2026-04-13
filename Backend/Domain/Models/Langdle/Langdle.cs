using Domain.Constraints.Langdle;
using Domain.Enums;

namespace Domain.Models.Langdle;

public class Langdle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public int YearFirstAppeared { get; private set; }
    public TypingDiscipline TypingDiscipline { get; private set; }
    public TypeStrength TypeStrength { get; private set; }
    public ExecutionModel ExecutionModel { get; private set; }
    public MemoryManagement MemoryManagement { get; private set; }
    public List<string> Tags { get; private set; } = [];

    // Parameterless constructor for EF Core
    private Langdle() { }

    public Langdle(LangdleCreationParams creationParams)
    {
        LangdleGuard.ValidateName(creationParams.Name);
        LangdleGuard.ValidateYearFirstAppeared(creationParams.YearFirstAppeared);
        foreach (var tag in creationParams.Tags)
        {
            LangdleGuard.ValidateTag(tag);
        }

        Name = creationParams.Name;
        YearFirstAppeared = creationParams.YearFirstAppeared;
        TypingDiscipline = creationParams.TypingDiscipline;
        TypeStrength = creationParams.TypeStrength;
        ExecutionModel = creationParams.ExecutionModel;
        MemoryManagement = creationParams.MemoryManagement;
        Tags = creationParams.Tags;
    }

    public void Update(
        string? name = null,
        int? yearFirstAppeared = null,
        TypingDiscipline? typingDiscipline = null,
        TypeStrength? typeStrength = null,
        ExecutionModel? executionModel = null,
        MemoryManagement? memoryManagement = null)
    {
        if (name is not null)
        {
            LangdleGuard.ValidateName(name);
            Name = name;
        }

        if (yearFirstAppeared is not null)
        {
            LangdleGuard.ValidateYearFirstAppeared(yearFirstAppeared.Value);
            YearFirstAppeared = yearFirstAppeared.Value;
        }

        if (typingDiscipline is not null)
        {
            TypingDiscipline = typingDiscipline.Value;
        }

        if (typeStrength is not null)
        {
            TypeStrength = typeStrength.Value;
        }

        if (executionModel is not null)
        {
            ExecutionModel = executionModel.Value;
        }

        if (memoryManagement is not null)
        {
            MemoryManagement = memoryManagement.Value;
        }
    }

    public void AddTag(string tag)
    {
        LangdleGuard.ValidateTag(tag);
        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
        }
    }

    public void RemoveTag(string tag)
    {
        LangdleGuard.ValidateTag(tag);
        Tags.Remove(tag);
    }
}
