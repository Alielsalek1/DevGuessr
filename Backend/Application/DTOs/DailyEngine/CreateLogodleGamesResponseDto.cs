using Application.DTOs.LogodlePlayer;

namespace Application.DTOs.LogodlePlayer;

public class CreateLogodleGamesResponseDto
{
    public List<CreateLogodleGameResponseDto> Items { get; set; } = [];
    public int CreatedCount => Items.Count;
}