using API.Extensions;
using Application.DTOs.LogodleTarget;
using Application.Utils;
using Asp.Versioning;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Techdle.Application.Services.Interfaces.LogodleTarget;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/logodle-targets")]
public class LogodleTargetsController(ILogodleTargetService logodleTargetService) : ControllerBase
{
    private readonly ILogodleTargetService _logodleTargetService = logodleTargetService;

    [HttpGet]
    [ProducesResponseType(typeof(SuccessApiResponse<GetPagedLogodleTargetsResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] GetPagedLogodleTargetsRequestDto request, CancellationToken ct)
    {
        var result = await _logodleTargetService.GetPagedAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(typeof(SuccessApiResponse<GetLogodleTargetByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name, CancellationToken ct)
    {
        var result = await _logodleTargetService.GetByNameAsync(name, ct);
        return this.ToActionResult(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SuccessApiResponse<CreateLogodleTargetResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromForm] CreateLogodleTargetRequestDto request, CancellationToken ct)
    {
        var result = await _logodleTargetService.CreateAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(typeof(SuccessApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string name, CancellationToken ct)
    {
        var result = await _logodleTargetService.DeleteByNameAsync(name, ct);
        return this.ToActionResult(result);
    }
}
