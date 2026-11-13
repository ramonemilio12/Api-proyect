using Microsoft.AspNetCore.Mvc;
using BiciTaller.Domain.Entities;
using BiciTaller.Infrastructure.Exceptions;
using BiciTaller.Infrastructure.Interfaces;
using BiciTaller.Infrastructure.Models;

namespace BiciTaller.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public sealed class CategoriasController(ICategoriaRepository categorias)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoriaModel>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await categorias.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoriaModel>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var categoria = await categorias.GetByIdAsync(id, cancellationToken);
        return categoria is null ? NotFound() : Ok(ToModel(categoria));
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaModel>> Create(
        CategoriaWriteModel model,
        CancellationToken cancellationToken)
    {
        var categoria = new Categoria
        {
            Codigo = NormalizeCodigo(model.Codigo),
            Nombre = model.Nombre.Trim(),
            Descripcion = model.Descripcion?.Trim(),
        };
        try
        {
            await categorias.AddAsync(categoria, cancellationToken);
        }
        catch (CategoriaException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, ToModel(categoria));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoriaModel>> Update(
        Guid id,
        CategoriaWriteModel model,
        CancellationToken cancellationToken)
    {
        var categoria = await categorias.GetByIdAsync(id, cancellationToken);
        if (categoria is null)
        {
            return NotFound();
        }

        categoria.Codigo = NormalizeCodigo(model.Codigo);
        categoria.Nombre = model.Nombre.Trim();
        categoria.Descripcion = model.Descripcion?.Trim();
        try
        {
            await categorias.UpdateAsync(categoria, cancellationToken);
        }
        catch (CategoriaException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return Ok(ToModel(categoria));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await categorias.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static string NormalizeCodigo(string codigo) => codigo.Trim().ToUpperInvariant();

    private static CategoriaModel ToModel(Categoria categoria) =>
        new(
            categoria.Id,
            categoria.Codigo,
            categoria.Nombre,
            categoria.Descripcion,
            categoria.CreatedAtUtc,
            categoria.UpdatedAtUtc);
}
