using API.Extensions;
using Application.DTOs.ProgrammingLanguage;
using Application.Utils;
using Asp.Versioning;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevGuessr.Application.Services.Interfaces.ProgrammingLanguage;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/programming-languages")]
public class ProgrammingLanguagesController(IProgrammingLanguageService programmingLanguageService) : ControllerBase
{
    private readonly IProgrammingLanguageService _programmingLanguageService = programmingLanguageService;

    [HttpGet]
    [ProducesResponseType(typeof(SuccessApiResponse<GetPagedProgrammingLanguagesResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] GetPagedProgrammingLanguagesRequestDto request, CancellationToken ct)
    {
        var result = await _programmingLanguageService.GetPagedAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpGet("{name}")]
    [ProducesResponseType(typeof(SuccessApiResponse<GetProgrammingLanguageByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name, CancellationToken ct)
    {
        var result = await _programmingLanguageService.GetByNameAsync(name, ct);
        return this.ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SuccessApiResponse<CreateProgrammingLanguageResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateProgrammingLanguageRequestDto request, CancellationToken ct)
    {
        var result = await _programmingLanguageService.CreateAsync(request, ct);
        return this.ToActionResult(result);
    }

    [HttpPut("{name}")]
    [ProducesResponseType(typeof(SuccessApiResponse<UpdateProgrammingLanguageByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(string name, [FromBody] UpdateProgrammingLanguageByNameRequestDto request, CancellationToken ct)
    {
        var result = await _programmingLanguageService.UpdateByNameAsync(name, request, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("{name}/tags")]
    [ProducesResponseType(typeof(SuccessApiResponse<AddProgrammingLanguageTagByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTag(string name, [FromBody] AddProgrammingLanguageTagByNameRequestDto request, CancellationToken ct)
    {
        var result = await _programmingLanguageService.AddTagByNameAsync(name, request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{name}/tags")]
    [ProducesResponseType(typeof(SuccessApiResponse<RemoveProgrammingLanguageTagByNameResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTag(string name, [FromBody] RemoveProgrammingLanguageTagByNameRequestDto request, CancellationToken ct)
    {
        var result = await _programmingLanguageService.RemoveTagByNameAsync(name, request, ct);
        return this.ToActionResult(result);
    }

    [HttpDelete("{name}")]
    [ProducesResponseType(typeof(SuccessApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string name, CancellationToken ct)
    {
        var result = await _programmingLanguageService.DeleteByNameAsync(name, ct);
        return this.ToActionResult(result);
    }
}
