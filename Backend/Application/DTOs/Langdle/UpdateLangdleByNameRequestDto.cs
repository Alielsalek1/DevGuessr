namespace Application.DTOs.Langdle;

public record UpdateLangdleByNameRequestDto
{
    public string? Name { get; init; }
    public int? YearFirstAppeared { get; init; }
    public string? TypingDiscipline { get; init; }
    public string? TypeStrength { get; init; }
    public string? ExecutionModel { get; init; }
    public string? MemoryManagement { get; init; }
}
