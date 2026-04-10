using Microsoft.AspNetCore.Http;

namespace Application.DTOs.LogodleTarget;

public record CreateLogodleTargetRequestDto
{
    public string Name { get; init; } = null!;
    public IFormFile Image { get; init; } = null!;
}
