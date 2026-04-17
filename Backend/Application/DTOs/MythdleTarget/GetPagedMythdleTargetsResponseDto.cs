namespace Application.DTOs.MythdleTarget;

public record GetPagedMythdleTargetsResponseDto
{
    public required IEnumerable<MythdleTargetDto> Targets { get; init; }
}
