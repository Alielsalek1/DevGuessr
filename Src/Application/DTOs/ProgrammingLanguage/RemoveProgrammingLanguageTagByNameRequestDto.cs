namespace Application.DTOs.ProgrammingLanguage;

public record RemoveProgrammingLanguageTagByNameRequestDto
{
    public string Tag { get; init; } = null!;
}
