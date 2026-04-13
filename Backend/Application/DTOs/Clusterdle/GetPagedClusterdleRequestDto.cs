namespace Application.DTOs.Clusterdle;

public record GetPagedClusterdleRequestDto
{
    public int PageNumber { get; init; } = 1;

    private readonly int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > 100 ? 100 : (value < 1 ? 1 : value);
    }
}
