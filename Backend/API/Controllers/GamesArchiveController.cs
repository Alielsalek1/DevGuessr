using API.Extensions;
using Application.DTOs.Archive;
using Application.Services.Interfaces;
using Application.Utils;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/games")]
public class GamesArchiveController(IGameArchiveService gameArchiveService) : ControllerBase
{
    private readonly IGameArchiveService _gameArchiveService = gameArchiveService;

    [HttpGet("archive")]
    [ProducesResponseType(typeof(SuccessApiResponse<GetPastGamesResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPastGames([FromQuery] GetPastGamesRequestDto request, CancellationToken ct)
    {
        var result = await _gameArchiveService.GetPastGamesAsync(request, ct);
        return this.ToActionResult(result);
    }
}
