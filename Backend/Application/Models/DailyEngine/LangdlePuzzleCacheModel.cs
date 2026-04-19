using Application.DTOs.LangdlePlayer;
using Domain.Enums;

namespace Application.Models;

public record LangdlePuzzleCacheModel
{
	public Guid PuzzleId { get; init; }
	public CreateLangdleGameResponseDto LoadInfo { get; init; } = null!;
	public Guid TargetId { get; init; }
	public int TargetYearFirstAppeared { get; init; }
	public TypeChecking TargetTypeChecking { get; init; }
	public Memory TargetMemory { get; init; }
	public ScopeSyntax TargetScopeSyntax { get; init; }
	public Semicolons TargetSemicolons { get; init; }
	public List<string> TargetTags { get; init; } = [];
}