using Domain.Enums;

namespace Domain.Models.Langdle;

public record LangdleCreationParams
{
    public required string Name { get; init; }
    public required int YearFirstAppeared { get; init; }
    public required TypingDiscipline TypingDiscipline { get; init; }
    public required TypeStrength TypeStrength { get; init; }
    public required ExecutionModel ExecutionModel { get; init; }
    public required MemoryManagement MemoryManagement { get; init; }
    public required List<string> Tags { get; init; }
}
