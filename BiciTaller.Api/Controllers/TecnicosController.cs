using Microsoft.AspNetCore.Mvc;
using BiciTaller.Application.Contract;
using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Tecnico;

namespace BiciTaller.Api.Controllers;

[ApiController]
[Route("api/tecnicos")]
public sealed class TecnicosController(ITecnicoService service)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TecnicoDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TecnicoDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Map(result);
    }

    [HttpPost]
    public async Task<ActionResult<TecnicoDto>> Create(
        TecnicoCreateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(dto, cancellationToken);
        if (!result.Success)
        {
            return Conflict(new { errors = result.Errors });
        }

        return CreatedAtAction(
            nameof(GetById), new { id = result.Data.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TecnicoDto>> Update(
        Guid id,
        TecnicoUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, dto, cancellationToken);
        return Map(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.Success ? NoContent() : NotFound(new { errors = result.Errors });
    }

    private ActionResult<TecnicoDto> Map(ServiceResult<TecnicoDto> result)
    {
        if (!result.Success)
        {
            return NotFound(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }
}
