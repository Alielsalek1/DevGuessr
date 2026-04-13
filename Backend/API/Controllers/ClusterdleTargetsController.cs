using API.Extensions;
using Application.DTOs.Clusterdle;
using Application.Services.Interfaces.Clusterdle;
using Application.Utils;
using Asp.Versioning;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/clusterdle")]
public class ClusterdleTargetsController(IClusterdleService clusterdleService) : ControllerBase
{
    private readonly IClusterdleService _clusterdleService = clusterdleService;

    [HttpGet]
    [ProducesResponseType(typeof(SuccessApiResponse<GetPagedClusterdleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] GetPagedClusterdleRequestDto request, CancellationToken ct)
    {
        var result = await _clusterdleService.GetPagedAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpGet("{groupName}")]
    [ProducesResponseType(typeof(SuccessApiResponse<GetClusterdleByGroupNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByGroupName(string groupName, CancellationToken ct)
    {
        var result = await _clusterdleService.GetByGroupNameAsync(groupName, ct);
        return this.ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SuccessApiResponse<CreateClusterdleResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateClusterdleRequestDto request, CancellationToken ct)
    {
        var result = await _clusterdleService.CreateAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpPut("{groupName}")]
    [ProducesResponseType(typeof(SuccessApiResponse<UpdateClusterdleByGroupNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(string groupName, [FromBody] UpdateClusterdleByGroupNameRequestDto request, CancellationToken ct)
    {
        var result = await _clusterdleService.UpdateByGroupNameAsync(groupName, request, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("{groupName}/words")]
    [ProducesResponseType(typeof(SuccessApiResponse<AddClusterdleWordByGroupNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddWord(string groupName, [FromBody] AddClusterdleWordByGroupNameRequestDto request, CancellationToken ct)
    {
        var result = await _clusterdleService.AddWordByGroupNameAsync(groupName, request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{groupName}/words")]
    [ProducesResponseType(typeof(SuccessApiResponse<RemoveClusterdleWordByGroupNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveWord(string groupName, [FromBody] RemoveClusterdleWordByGroupNameRequestDto request, CancellationToken ct)
    {
        var result = await _clusterdleService.RemoveWordByGroupNameAsync(groupName, request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{groupName}")]
    [ProducesResponseType(typeof(SuccessApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string groupName, CancellationToken ct)
    {
        var result = await _clusterdleService.DeleteByGroupNameAsync(groupName, ct);
        return this.ToActionResult(result);
    }
}
