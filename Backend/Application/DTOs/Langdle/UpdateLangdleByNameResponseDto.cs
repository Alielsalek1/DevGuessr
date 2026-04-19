namespace Application.DTOs.Langdle;

public record UpdateLangdleByNameResponseDto
{

    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int YearFirstAppeared { get; init; }
    public string TypeChecking { get; init; } = null!;
    public string Memory { get; init; } = null!;
    public string ScopeSyntax { get; init; } = null!;
    public string Semicolons { get; init; } = null!;
    public List<string> Tags { get; init; } = [];
}
