using API.Extensions;
using Application.DTOs.Langdle;
using Application.Services.Interfaces.Langdle;
using Application.Utils;
using Asp.Versioning;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/langdle")]
public class LangdleTargetsController(ILangdleService langdleService) : ControllerBase
{
    private readonly ILangdleService _langdleService = langdleService;

    [HttpGet]
    [ProducesResponseType(typeof(SuccessApiResponse<GetPagedLangdleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] GetPagedLangdleRequestDto request, CancellationToken ct)
    {
        var result = await _langdleService.GetPagedAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(typeof(SuccessApiResponse<GetLangdleByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name, CancellationToken ct)
    {
        var result = await _langdleService.GetByNameAsync(name, ct);
        return this.ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SuccessApiResponse<CreateLangdleResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateLangdleRequestDto request, CancellationToken ct)
    {
        var result = await _langdleService.CreateAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpPut("{name}")]
    [ProducesResponseType(typeof(SuccessApiResponse<UpdateLangdleByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(string name, [FromBody] UpdateLangdleByNameRequestDto request, CancellationToken ct)
    {
        var result = await _langdleService.UpdateByNameAsync(name, request, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("{name}/tags")]
    [ProducesResponseType(typeof(SuccessApiResponse<AddLangdleTagByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTag(string name, [FromBody] AddLangdleTagByNameRequestDto request, CancellationToken ct)
    {
        var result = await _langdleService.AddTagByNameAsync(name, request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{name}/tags")]
    [ProducesResponseType(typeof(SuccessApiResponse<RemoveLangdleTagByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTag(string name, [FromBody] RemoveLangdleTagByNameRequestDto request, CancellationToken ct)
    {
        var result = await _langdleService.RemoveTagByNameAsync(name, request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(typeof(SuccessApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string name, CancellationToken ct)
    {
        var result = await _langdleService.DeleteByNameAsync(name, ct);
        return this.ToActionResult(result);
    }
}
