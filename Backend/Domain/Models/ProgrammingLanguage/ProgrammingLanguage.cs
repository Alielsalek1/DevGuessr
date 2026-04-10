using Domain.Constraints.ProgrammingLanguage;
using Domain.Enums;

namespace Domain.Models.ProgrammingLanguage;

public class ProgrammingLanguage
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
    private ProgrammingLanguage() { }

    public ProgrammingLanguage(ProgrammingLanguageCreationParams creationParams)
    {
        ProgrammingLanguageGuard.ValidateName(creationParams.Name);
        ProgrammingLanguageGuard.ValidateYearFirstAppeared(creationParams.YearFirstAppeared);
        foreach (var tag in creationParams.Tags)
        {
            ProgrammingLanguageGuard.ValidateTag(tag);
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
            ProgrammingLanguageGuard.ValidateName(name);
            Name = name;
        }

        if (yearFirstAppeared is not null)
        {
            ProgrammingLanguageGuard.ValidateYearFirstAppeared(yearFirstAppeared.Value);
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
        ProgrammingLanguageGuard.ValidateTag(tag);
        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
        }
    }

    public void RemoveTag(string tag)
    {
        ProgrammingLanguageGuard.ValidateTag(tag);
        Tags.Remove(tag);
    }
}
