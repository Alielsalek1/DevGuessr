using API.Extensions;
using Application.DTOs.TechdlePlayer;
using Application.Services.Interfaces;
using Application.Utils;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/techdle")]
public class TechdleController(ITechdlePlayerService techdlePlayerService) : ControllerBase
{
    private readonly ITechdlePlayerService _techdlePlayerService = techdlePlayerService;

    [HttpPost("guess")]
    [ProducesResponseType(typeof(SuccessApiResponse<TechdleGuessResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Guess([FromBody] TechdleGuessRequestDto request, CancellationToken ct)
    {
        var result = await _techdlePlayerService.EvaluateGuessAsync(request, ct);
        return this.ToActionResult(result);
    }
}
