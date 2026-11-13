using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using BiciTaller.Domain.Entities;
using BiciTaller.Domain.Repository;
using BiciTaller.Infrastructure.Interfaces;

namespace BiciTaller.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController(
    IRepository<Cliente> clientes,
    ICategoriaRepository categorias) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Cliente>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await clientes.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Cliente>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cliente = await clientes.GetByIdAsync(id, cancellationToken);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> Create(
        ClienteRequest model,
        CancellationToken cancellationToken)
    {
        if (await categorias.GetByIdAsync(model.CategoriaId, cancellationToken) is null)
        {
            return BadRequest(new { message = "La categoría indicada no existe." });
        }

        var cliente = new Cliente();
        Apply(cliente, model);
        try
        {
            await clientes.AddAsync(cliente, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Cliente>> Update(
        Guid id,
        ClienteRequest model,
        CancellationToken cancellationToken)
    {
        var cliente = await clientes.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return NotFound();
        }

        if (await categorias.GetByIdAsync(model.CategoriaId, cancellationToken) is null)
        {
            return BadRequest(new { message = "La categoría indicada no existe." });
        }

        Apply(cliente, model);
        try
        {
            await clientes.UpdateAsync(cliente, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return Ok(cliente);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await clientes.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static void Apply(Cliente cliente, ClienteRequest model)
    {
        cliente.NumeroCliente = model.NumeroCliente.Trim().ToUpperInvariant();
        cliente.Nombre = model.Nombre.Trim();
        cliente.Apellido = model.Apellido.Trim();
        cliente.Correo = model.Correo.Trim().ToLowerInvariant();
        cliente.Telefono = model.Telefono?.Trim();
        cliente.CategoriaId = model.CategoriaId;
    }
}

public sealed class ClienteRequest
{
    [Required, StringLength(30, MinimumLength = 2)]
    public string NumeroCliente { get; init; } = string.Empty;

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
