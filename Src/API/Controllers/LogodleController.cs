using API.Extensions;
using Application.DTOs.LogodlePlayer;
using Application.Services.Interfaces;
using Application.Utils;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/logodle")]
public class LogodleController(ILogodlePlayerService logodlePlayerService) : ControllerBase
{
    private readonly ILogodlePlayerService _logodlePlayerService = logodlePlayerService;

    [HttpPost("guess")]
    [ProducesResponseType(typeof(SuccessApiResponse<LogodleGuessResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Guess([FromBody] LogodleGuessRequestDto request, CancellationToken ct)
    {
        var result = await _logodlePlayerService.EvaluateGuessAsync(request, ct);
        return this.ToActionResult(result);
    }
}