using Application.DTOs.TechdlePlayer;
using Domain.Enums;

namespace Application.Models;

public record TechdlePuzzleCacheModel
{
	public Guid PuzzleId { get; init; }
	public TechdleLoadDto LoadInfo { get; init; } = null!;
	public Guid TargetId { get; init; }
	public int TargetYearFirstAppeared { get; init; }
	public TypingDiscipline TargetTypingDiscipline { get; init; }
	public TypeStrength TargetTypeStrength { get; init; }
	public List<string> TargetTags { get; init; } = [];
}