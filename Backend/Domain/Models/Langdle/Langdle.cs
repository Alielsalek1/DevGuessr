using Domain.Constraints.Langdle;
using Domain.Enums;

namespace Domain.Models.Langdle;

public class Langdle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public int YearFirstAppeared { get; private set; }
    public TypeChecking TypeChecking { get; private set; }
    public Memory Memory { get; private set; }
    public ScopeSyntax ScopeSyntax { get; private set; }
    public Semicolons Semicolons { get; private set; }
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
        TypeChecking = creationParams.TypeChecking;
        Memory = creationParams.Memory;
        ScopeSyntax = creationParams.ScopeSyntax;
        Semicolons = creationParams.Semicolons;
        Tags = creationParams.Tags;
    }

    public void Update(
        string? name = null,
        int? yearFirstAppeared = null,
        TypeChecking? typeChecking = null,
        Memory? memory = null,
        ScopeSyntax? scopeSyntax = null,
        Semicolons? semicolons = null)
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

        if (typeChecking is not null)
        {
            TypeChecking = typeChecking.Value;
        }

        if (memory is not null)
        {
            Memory = memory.Value;
        }

        if (scopeSyntax is not null)
        {
            ScopeSyntax = scopeSyntax.Value;
        }

        if (semicolons is not null)
        {
            Semicolons = semicolons.Value;
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
