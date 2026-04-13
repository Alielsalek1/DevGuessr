namespace Application.DTOs.Langdle;

public record AddLangdleTagByNameResponseDto
{

    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int YearFirstAppeared { get; init; }
    public string TypingDiscipline { get; init; } = null!;
    public string TypeStrength { get; init; } = null!;
    public string ExecutionModel { get; init; } = null!;
    public string MemoryManagement { get; init; } = null!;
    public List<string> Tags { get; init; } = [];
}
