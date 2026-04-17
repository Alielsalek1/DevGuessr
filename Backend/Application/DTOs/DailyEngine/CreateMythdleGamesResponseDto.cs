using Application.DTOs.MythdlePlayer;

namespace Application.DTOs.MythdlePlayer;

public class CreateMythdleGamesResponseDto
{
    public List<CreateMythdleGameResponseDto> Items { get; set; } = [];
    public int CreatedCount => Items.Count;
}
