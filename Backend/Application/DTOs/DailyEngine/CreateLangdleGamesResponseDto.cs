using Application.DTOs.LangdlePlayer;

namespace Application.DTOs.LangdlePlayer;

public class CreateLangdleGamesResponseDto
{
    public List<CreateLangdleGameResponseDto> Items { get; set; } = [];
    public int CreatedCount => Items.Count;
}