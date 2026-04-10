using API.Extensions;
using Application.DTOs.TechnectionCategory;
using Application.Utils;
using Asp.Versioning;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Techdle.Application.Services.Interfaces.TechnectionCategory;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/technection-categories")]
public class TechnectionCategoriesController(ITechnectionCategoryService technectionCategoryService) : ControllerBase
{
    private readonly ITechnectionCategoryService _technectionCategoryService = technectionCategoryService;

    [HttpGet]
    [ProducesResponseType(typeof(SuccessApiResponse<GetPagedTechnectionCategoriesResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] GetPagedTechnectionCategoriesRequestDto request, CancellationToken ct)
    {
        var result = await _technectionCategoryService.GetPagedAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpGet("{groupName}")]
    [ProducesResponseType(typeof(SuccessApiResponse<GetTechnectionCategoryByGroupNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByGroupName(string groupName, CancellationToken ct)
    {
        var result = await _technectionCategoryService.GetByGroupNameAsync(groupName, ct);
        return this.ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SuccessApiResponse<CreateTechnectionCategoryResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateTechnectionCategoryRequestDto request, CancellationToken ct)
    {
        var result = await _technectionCategoryService.CreateAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpPut("{groupName}")]
    [ProducesResponseType(typeof(SuccessApiResponse<UpdateTechnectionCategoryByGroupNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(string groupName, [FromBody] UpdateTechnectionCategoryByGroupNameRequestDto request, CancellationToken ct)
    {
        var result = await _technectionCategoryService.UpdateByGroupNameAsync(groupName, request, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("{groupName}/words")]
    [ProducesResponseType(typeof(SuccessApiResponse<AddTechnectionCategoryWordByGroupNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddWord(string groupName, [FromBody] AddTechnectionCategoryWordByGroupNameRequestDto request, CancellationToken ct)
    {
        var result = await _technectionCategoryService.AddWordByGroupNameAsync(groupName, request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{groupName}/words")]
    [ProducesResponseType(typeof(SuccessApiResponse<RemoveTechnectionCategoryWordByGroupNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveWord(string groupName, [FromBody] RemoveTechnectionCategoryWordByGroupNameRequestDto request, CancellationToken ct)
    {
        var result = await _technectionCategoryService.RemoveWordByGroupNameAsync(groupName, request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{groupName}")]
    [ProducesResponseType(typeof(SuccessApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string groupName, CancellationToken ct)
    {
        var result = await _technectionCategoryService.DeleteByGroupNameAsync(groupName, ct);
        return this.ToActionResult(result);
    }
}
