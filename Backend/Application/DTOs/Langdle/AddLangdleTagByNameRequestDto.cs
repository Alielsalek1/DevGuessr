namespace Application.DTOs.Langdle;

public record AddLangdleTagByNameRequestDto
{
    public string Tag { get; init; } = null!;
}
