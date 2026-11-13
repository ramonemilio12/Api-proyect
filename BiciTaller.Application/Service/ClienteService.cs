using BiciTaller.Application.Contract;
using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Cliente;
using BiciTaller.Domain.Entities;
using BiciTaller.Domain.Repository;
using BiciTaller.Infrastructure.Interfaces;

namespace BiciTaller.Application.Service;

public sealed class ClienteService(
    IRepository<Cliente> clientes,
    ICategoriaRepository categorias)
    : BaseService, IClienteService
{
    public async Task<ServiceResult<IReadOnlyList<ClienteDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await clientes.GetAllAsync(cancellationToken);
        return Success<IReadOnlyList<ClienteDto>>(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ClienteDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var cliente = await clientes.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return Failure<ClienteDto>("Cliente no encontrado.");
        }

        return Success(ToDto(cliente));
    }

    public async Task<ServiceResult<ClienteDto>> CreateAsync(
        ClienteCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<ClienteDto>(validationErrors.ToArray());
        }

        var categoria = await categorias.GetByIdAsync(dto.CategoriaId, cancellationToken);
        if (categoria is null)
        {
            return Failure<ClienteDto>("La categoría indicada no existe.");
        }

        var numeroCliente = Normalize(dto.NumeroCliente);
        if (await clientes.ExistsAsync(
            c => c.NumeroCliente == numeroCliente, cancellationToken))
        {
            return Failure<ClienteDto>(
                $"Ya existe un cliente con el número '{numeroCliente}'.");
        }

        var email = dto.Correo.Trim().ToLowerInvariant();
        if (await clientes.ExistsAsync(c => c.Correo == email, cancellationToken))
        {
            return Failure<ClienteDto>($"Ya existe un cliente con el correo '{email}'.");
        }

        var cliente = new Cliente
        {
            NumeroCliente = numeroCliente,
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Correo = email,
            Telefono = string.IsNullOrWhiteSpace(dto.Telefono)
                ? null
                : dto.Telefono.Trim(),
            CategoriaId = dto.CategoriaId,
        };

        await clientes.AddAsync(cliente, cancellationToken);
        return Success(ToDto(cliente));
    }

    public async Task<ServiceResult<ClienteDto>> UpdateAsync(
        Guid id,
        ClienteUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<ClienteDto>(validationErrors.ToArray());
        }

        var cliente = await clientes.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return Failure<ClienteDto>("Cliente no encontrado.");
        }

        var categoria = await categorias.GetByIdAsync(dto.CategoriaId, cancellationToken);
        if (categoria is null)
        {
            return Failure<ClienteDto>("La categoría indicada no existe.");
        }

        var numeroCliente = Normalize(dto.NumeroCliente);
        if (await clientes.ExistsAsync(
            c => c.NumeroCliente == numeroCliente && c.Id != id, cancellationToken))
        {
            return Failure<ClienteDto>(
                $"Ya existe un cliente con el número '{numeroCliente}'.");
        }

        var email = dto.Correo.Trim().ToLowerInvariant();
        if (await clientes.ExistsAsync(
            c => c.Correo == email && c.Id != id, cancellationToken))
        {
            return Failure<ClienteDto>($"Ya existe un cliente con el correo '{email}'.");
        }

        cliente.NumeroCliente = numeroCliente;
        cliente.Nombre = dto.Nombre.Trim();
        cliente.Apellido = dto.Apellido.Trim();
        cliente.Correo = email;
        cliente.Telefono = string.IsNullOrWhiteSpace(dto.Telefono)
            ? null
            : dto.Telefono.Trim();
        cliente.CategoriaId = dto.CategoriaId;
        cliente.MarkUpdated();

        await clientes.UpdateAsync(cliente, cancellationToken);
        return Success(ToDto(cliente));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await clientes.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Failure<bool>("Cliente no encontrado.");
        }

        return Success(true);
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();

    private static ClienteDto ToDto(Cliente cliente) =>
        new()
        {
            Id = cliente.Id,
            NumeroCliente = cliente.NumeroCliente,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            NombreCompleto = cliente.NombreCompleto,
            Correo = cliente.Correo,
            Telefono = cliente.Telefono,
            CategoriaId = cliente.CategoriaId,
            NombreCategoria = cliente.Categoria?.Nombre ?? string.Empty,
            FechaRegistroUtc = cliente.FechaRegistroUtc,
            CreatedAtUtc = cliente.CreatedAtUtc,
            UpdatedAtUtc = cliente.UpdatedAtUtc,
        };
}
