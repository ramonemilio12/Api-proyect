using Microsoft.AspNetCore.Mvc;
using BiciTaller.Domain.Entities;
using BiciTaller.Domain.Repository;
using BiciTaller.Infrastructure.Exceptions;
using BiciTaller.Infrastructure.Interfaces;
using BiciTaller.Infrastructure.Models;

namespace BiciTaller.Api.Controllers;

[ApiController]
[Route("api/servicios")]
public sealed class ServiciosController(
    IServicioRepository servicios,
    ICategoriaRepository categorias,
    IRepository<Tecnico> tecnicos) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ServicioModel>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await servicios.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServicioModel>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var servicio = await servicios.GetByIdAsync(id, cancellationToken);
        return servicio is null ? NotFound() : Ok(ToModel(servicio));
    }

    [HttpPost]
    public async Task<ActionResult<ServicioModel>> Create(
        ServicioWriteModel model,
        CancellationToken cancellationToken)
    {
        var invalidReference = await ValidateReferencesAsync(model, cancellationToken);
        if (invalidReference is not null)
        {
            return BadRequest(new { message = invalidReference });
        }

        var servicio = new Servicio();
        Apply(servicio, model);
        try
        {
            await servicios.AddAsync(servicio, cancellationToken);
        }
        catch (ServicioException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var created = await servicios.GetByIdAsync(servicio.Id, cancellationToken);
        return CreatedAtAction(
            nameof(GetById), new { id = servicio.Id }, ToModel(created!));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ServicioModel>> Update(
        Guid id,
        ServicioWriteModel model,
        CancellationToken cancellationToken)
    {
        var servicio = await servicios.GetByIdAsync(id, cancellationToken);
        if (servicio is null)
        {
            return NotFound();
        }

        var invalidReference = await ValidateReferencesAsync(model, cancellationToken);
        if (invalidReference is not null)
        {
            return BadRequest(new { message = invalidReference });
        }

        Apply(servicio, model);
        try
        {
            await servicios.UpdateAsync(servicio, cancellationToken);
        }
        catch (ServicioException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var updated = await servicios.GetByIdAsync(id, cancellationToken);
        return Ok(ToModel(updated!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await servicios.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private async Task<string?> ValidateReferencesAsync(
        ServicioWriteModel model,
        CancellationToken cancellationToken)
    {
        var categoria = await categorias.GetByIdAsync(
            model.CategoriaId, cancellationToken);
        if (categoria is null)
        {
            return "La categoría indicada no existe.";
        }

        if (model.TecnicoId is not Guid tecnicoId)
        {
            return null;
        }

        var tecnico = await tecnicos.GetByIdAsync(
            tecnicoId, cancellationToken);
        if (tecnico is null)
        {
            return "El técnico indicado no existe.";
        }

        return tecnico.CategoriaId == model.CategoriaId
            ? null
            : "El técnico debe pertenecer a la misma categoría del servicio.";
    }

    private static void Apply(Servicio servicio, ServicioWriteModel model)
    {
        servicio.Codigo = NormalizeCodigo(model.Codigo);
        servicio.Nombre = model.Nombre.Trim();
        servicio.Precio = model.Precio;
        servicio.CategoriaId = model.CategoriaId;
        servicio.TecnicoId = model.TecnicoId;
    }

    private static string NormalizeCodigo(string codigo) => codigo.Trim().ToUpperInvariant();

    private static ServicioModel ToModel(Servicio servicio) =>
        new(
            servicio.Id,
            servicio.Codigo,
            servicio.Nombre,
            servicio.Precio,
            servicio.CategoriaId,
            servicio.Categoria.Nombre,
            servicio.TecnicoId,
            servicio.Tecnico?.NombreCompleto,
            servicio.CreatedAtUtc,
            servicio.UpdatedAtUtc);
}
