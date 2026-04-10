namespace Application.DTOs.ProgrammingLanguage;

public record UpdateProgrammingLanguageByNameRequestDto
{
    public string? Name { get; init; }
    public int? YearFirstAppeared { get; init; }
    public string? TypingDiscipline { get; init; }
    public string? TypeStrength { get; init; }
    public string? ExecutionModel { get; init; }
    public string? MemoryManagement { get; init; }
}
