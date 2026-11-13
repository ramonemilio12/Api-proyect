using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using BiciTaller.Domain.Entities;
using BiciTaller.Domain.Repository;
using BiciTaller.Infrastructure.Interfaces;

namespace BiciTaller.Api.Controllers;

[ApiController]
[Route("api/tecnicos")]
public sealed class TecnicosController(
    IRepository<Tecnico> tecnicos,
    ICategoriaRepository categorias) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Tecnico>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await tecnicos.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Tecnico>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tecnico = await tecnicos.GetByIdAsync(id, cancellationToken);
        return tecnico is null ? NotFound() : Ok(tecnico);
    }

    [HttpPost]
    public async Task<ActionResult<Tecnico>> Create(
        TecnicoRequest model,
        CancellationToken cancellationToken)
    {
        if (await categorias.GetByIdAsync(model.CategoriaId, cancellationToken) is null)
        {
            return BadRequest(new { message = "La categoría indicada no existe." });
        }

        var tecnico = new Tecnico();
        Apply(tecnico, model);
        try
        {
            await tecnicos.AddAsync(tecnico, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = tecnico.Id }, tecnico);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Tecnico>> Update(
        Guid id,
        TecnicoRequest model,
        CancellationToken cancellationToken)
    {
        var tecnico = await tecnicos.GetByIdAsync(id, cancellationToken);
        if (tecnico is null)
        {
            return NotFound();
        }

        if (await categorias.GetByIdAsync(model.CategoriaId, cancellationToken) is null)
        {
            return BadRequest(new { message = "La categoría indicada no existe." });
        }

        Apply(tecnico, model);
        try
        {
            await tecnicos.UpdateAsync(tecnico, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return Ok(tecnico);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await tecnicos.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static void Apply(Tecnico tecnico, TecnicoRequest model)
    {
        tecnico.NumeroEmpleado = model.NumeroEmpleado.Trim().ToUpperInvariant();
        tecnico.Nombre = model.Nombre.Trim();
        tecnico.Apellido = model.Apellido.Trim();
        tecnico.Correo = model.Correo.Trim().ToLowerInvariant();
        tecnico.Telefono = model.Telefono?.Trim();
        tecnico.CategoriaId = model.CategoriaId;
    }
}

public sealed class TecnicoRequest
{
    [Required, StringLength(30, MinimumLength = 2)]
    public string NumeroEmpleado { get; init; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 1)]
    public string Nombre { get; init; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 1)]
    public string Apellido { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Correo { get; init; } = string.Empty;

    [Phone, StringLength(30)]
    public string? Telefono { get; init; }

    public Guid CategoriaId { get; init; }
}
