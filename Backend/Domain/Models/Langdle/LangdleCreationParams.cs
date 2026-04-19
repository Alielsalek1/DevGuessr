using Domain.Enums;

namespace Domain.Models.Langdle;

public record LangdleCreationParams
{
    public required string Name { get; init; }
    public required int YearFirstAppeared { get; init; }
    public required TypeChecking TypeChecking { get; init; }
    public required Memory Memory { get; init; }
    public required ScopeSyntax ScopeSyntax { get; init; }
    public required Semicolons Semicolons { get; init; }
    public required List<string> Tags { get; init; }
}
