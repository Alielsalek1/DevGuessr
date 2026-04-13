namespace Application.DTOs.Langdle;

public record RemoveLangdleTagByNameRequestDto
{
    public string Tag { get; init; } = null!;
}
