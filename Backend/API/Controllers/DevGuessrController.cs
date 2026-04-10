using API.Extensions;
using Application.DTOs.DevGuessrPlayer;
using Application.Services.Interfaces;
using Application.Utils;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/devguessr")]
public class DevGuessrController(IDevGuessrPlayerService devGuessrPlayerService) : ControllerBase
{
    private readonly IDevGuessrPlayerService _devGuessrPlayerService = devGuessrPlayerService;

    [HttpGet("games/by-date")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<DevGuessrGameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameByDate([FromQuery] DateOnly date, CancellationToken ct)
    {
        var result = await _devGuessrPlayerService.GetGameByDateAsync(date, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("guess")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SuccessApiResponse<DevGuessrGuessResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Guess([FromBody] DevGuessrGuessRequestDto request, CancellationToken ct)
    {
        var result = await _devGuessrPlayerService.EvaluateGuessAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("games")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SuccessApiResponse<CreateDevGuessrGameResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateGame(CancellationToken ct)
    {
        var result = await _devGuessrPlayerService.CreateGameAsync(ct);
        return this.ToActionResult(result);
    }
}
