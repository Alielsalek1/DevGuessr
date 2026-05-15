namespace Application.DTOs.Archive;

public record GetPastGamesRequestDto
{
    public int PageNumber { get; init; } = 1;

    private readonly int _pageSize = 30;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > 50 ? 50 : (value < 1 ? 1 : value);
    }
}
