namespace Application.DTOs.ProgrammingLanguage;

public record AddProgrammingLanguageTagByNameRequestDto
{
    public string Tag { get; init; } = null!;
}
