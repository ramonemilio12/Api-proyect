using Microsoft.AspNetCore.Mvc;
using BiciTaller.Application.Contract;
using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Servicio;

namespace BiciTaller.Api.Controllers;

[ApiController]
[Route("api/servicios")]
public sealed class ServiciosController(IServicioService service)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ServicioDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServicioDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Map(result);
    }

    [HttpPost]
    public async Task<ActionResult<ServicioDto>> Create(
        ServicioCreateDto dto,
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
    public async Task<ActionResult<ServicioDto>> Update(
        Guid id,
        ServicioUpdateDto dto,
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

    private ActionResult<ServicioDto> Map(ServiceResult<ServicioDto> result)
    {
        if (!result.Success)
        {
            return NotFound(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }
}
