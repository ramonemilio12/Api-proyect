using BiciTaller.Application.Contract;
using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Tecnico;
using BiciTaller.Domain.Entities;
using BiciTaller.Domain.Repository;
using BiciTaller.Infrastructure.Interfaces;

namespace BiciTaller.Application.Service;

public sealed class TecnicoService(
    IRepository<Tecnico> tecnicos,
    ICategoriaRepository categorias)
    : BaseService, ITecnicoService
{
    public async Task<ServiceResult<IReadOnlyList<TecnicoDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await tecnicos.GetAllAsync(cancellationToken);
        return Success<IReadOnlyList<TecnicoDto>>(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<TecnicoDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var tecnico = await tecnicos.GetByIdAsync(id, cancellationToken);
        if (tecnico is null)
        {
            return Failure<TecnicoDto>("Tecnico no encontrado.");
        }

        return Success(ToDto(tecnico));
    }

    public async Task<ServiceResult<TecnicoDto>> CreateAsync(
        TecnicoCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<TecnicoDto>(validationErrors.ToArray());
        }

        var categoria = await categorias.GetByIdAsync(dto.CategoriaId, cancellationToken);
        if (categoria is null)
        {
            return Failure<TecnicoDto>("La categoría indicada no existe.");
        }

        var numeroEmpleado = Normalize(dto.NumeroEmpleado);
        if (await tecnicos.ExistsAsync(
            t => t.NumeroEmpleado == numeroEmpleado, cancellationToken))
        {
            return Failure<TecnicoDto>(
                $"Ya existe un técnico con el número '{numeroEmpleado}'.");
        }

        var email = dto.Correo.Trim().ToLowerInvariant();
        if (await tecnicos.ExistsAsync(t => t.Correo == email, cancellationToken))
        {
            return Failure<TecnicoDto>(
                $"Ya existe un técnico con el correo '{email}'.");
        }

        var tecnico = new Tecnico
        {
            NumeroEmpleado = numeroEmpleado,
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Correo = email,
            Telefono = string.IsNullOrWhiteSpace(dto.Telefono)
                ? null
                : dto.Telefono.Trim(),
            CategoriaId = dto.CategoriaId,
        };

        await tecnicos.AddAsync(tecnico, cancellationToken);
        return Success(ToDto(tecnico));
    }

    public async Task<ServiceResult<TecnicoDto>> UpdateAsync(
        Guid id,
        TecnicoUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<TecnicoDto>(validationErrors.ToArray());
        }

        var tecnico = await tecnicos.GetByIdAsync(id, cancellationToken);
        if (tecnico is null)
        {
            return Failure<TecnicoDto>("Tecnico no encontrado.");
        }

        var categoria = await categorias.GetByIdAsync(dto.CategoriaId, cancellationToken);
        if (categoria is null)
        {
            return Failure<TecnicoDto>("La categoría indicada no existe.");
        }

        var numeroEmpleado = Normalize(dto.NumeroEmpleado);
        if (await tecnicos.ExistsAsync(
            t => t.NumeroEmpleado == numeroEmpleado && t.Id != id, cancellationToken))
        {
            return Failure<TecnicoDto>(
                $"Ya existe un técnico con el número '{numeroEmpleado}'.");
        }

        var email = dto.Correo.Trim().ToLowerInvariant();
        if (await tecnicos.ExistsAsync(
            t => t.Correo == email && t.Id != id, cancellationToken))
        {
            return Failure<TecnicoDto>(
                $"Ya existe un técnico con el correo '{email}'.");
        }

        tecnico.NumeroEmpleado = numeroEmpleado;
        tecnico.Nombre = dto.Nombre.Trim();
        tecnico.Apellido = dto.Apellido.Trim();
        tecnico.Correo = email;
        tecnico.Telefono = string.IsNullOrWhiteSpace(dto.Telefono)
            ? null
            : dto.Telefono.Trim();
        tecnico.CategoriaId = dto.CategoriaId;
        tecnico.MarkUpdated();

        await tecnicos.UpdateAsync(tecnico, cancellationToken);
        return Success(ToDto(tecnico));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await tecnicos.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Failure<bool>("Tecnico no encontrado.");
        }

        return Success(true);
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();

    private static TecnicoDto ToDto(Tecnico tecnico) =>
        new()
        {
            Id = tecnico.Id,
            NumeroEmpleado = tecnico.NumeroEmpleado,
            Nombre = tecnico.Nombre,
            Apellido = tecnico.Apellido,
            NombreCompleto = tecnico.NombreCompleto,
            Correo = tecnico.Correo,
            Telefono = tecnico.Telefono,
            CategoriaId = tecnico.CategoriaId,
            NombreCategoria = tecnico.Categoria?.Nombre ?? string.Empty,
            FechaIngresoUtc = tecnico.FechaIngresoUtc,
            CreatedAtUtc = tecnico.CreatedAtUtc,
            UpdatedAtUtc = tecnico.UpdatedAtUtc,
        };
}
