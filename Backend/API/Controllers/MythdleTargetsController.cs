using API.Extensions;
using Application.DTOs.MythdleTarget;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/mythdle-targets")]
public class MythdleTargetsController(IMythdleTargetService service) : ControllerBase
{
    private readonly IMythdleTargetService _service = service;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetTargetsAsync(pageNumber, pageSize);
        return this.ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMythdleTargetDto request)
    {
        var result = await _service.CreateTargetAsync(request);
        return this.ToActionResult(result);
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        var result = await _service.DeleteTargetByNameAsync(name);
        return this.ToActionResult(result);
    }
}
