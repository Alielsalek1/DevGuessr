using API.Extensions;
using Application.DTOs.MythdlePlayer;
using Application.Services.Interfaces;
using Application.Utils;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/mythdle")]
public class MythdleController(IMythdlePlayerService mythdlePlayerService) : ControllerBase
{
    private readonly IMythdlePlayerService _mythdlePlayerService = mythdlePlayerService;

    [HttpGet("games/by-date")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<MythdleGameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameByDate([FromQuery] DateOnly date, CancellationToken ct)
    {
        var result = await _mythdlePlayerService.GetGameByDateAsync(date, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("guess")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<MythdleGuessResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Guess([FromBody] MythdleGuessRequestDto request, CancellationToken ct)
    {
        var result = await _mythdlePlayerService.EvaluateGuessAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("games")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SuccessApiResponse<CreateMythdleGamesResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateGame(CancellationToken ct)
    {
        var result = await _mythdlePlayerService.CreateGamesAsync(ct);
        return this.ToActionResult(result);
    }
}
