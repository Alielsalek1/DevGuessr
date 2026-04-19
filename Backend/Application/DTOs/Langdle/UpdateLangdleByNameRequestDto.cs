namespace Application.DTOs.Langdle;

public record UpdateLangdleByNameRequestDto
{
    public string? Name { get; init; }
    public int? YearFirstAppeared { get; init; }
    public string? TypeChecking { get; init; }
    public string? Memory { get; init; }
    public string? ScopeSyntax { get; init; }
    public string? Semicolons { get; init; }
}
