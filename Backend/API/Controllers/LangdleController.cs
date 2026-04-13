using API.Extensions;
using Application.DTOs.LangdlePlayer;
using Application.Services.Interfaces;
using Application.Utils;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/langdle")]
public class LangdleController(ILangdlePlayerService langdlePlayerService) : ControllerBase
{
    private readonly ILangdlePlayerService _langdlePlayerService = langdlePlayerService;

    [HttpGet("games/by-date")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<LangdleGameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameByDate([FromQuery] DateOnly date, CancellationToken ct)
    {
        var result = await _langdlePlayerService.GetGameByDateAsync(date, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("guess")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<LangdleGuessResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Guess([FromBody] LangdleGuessRequestDto request, CancellationToken ct)
    {
        var result = await _langdlePlayerService.EvaluateGuessAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("games")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SuccessApiResponse<CreateLangdleGamesResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateGame(CancellationToken ct)
    {
        var result = await _langdlePlayerService.CreateGamesAsync(ct);
        return this.ToActionResult(result);
    }
}
