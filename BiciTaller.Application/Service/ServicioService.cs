using BiciTaller.Application.Contract;
using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Servicio;
using BiciTaller.Domain.Entities;
using BiciTaller.Domain.Repository;
using BiciTaller.Infrastructure.Interfaces;

namespace BiciTaller.Application.Service;

public sealed class ServicioService(
    IServicioRepository servicios,
    ICategoriaRepository categorias,
    IRepository<Tecnico> tecnicos)
    : BaseService, IServicioService
{
    public async Task<ServiceResult<IReadOnlyList<ServicioDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await servicios.GetAllAsync(cancellationToken);
        return Success<IReadOnlyList<ServicioDto>>(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<ServicioDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var servicio = await servicios.GetByIdAsync(id, cancellationToken);
        if (servicio is null)
        {
            return Failure<ServicioDto>("Servicio no encontrado.");
        }

        return Success(ToDto(servicio));
    }

    public async Task<ServiceResult<ServicioDto>> CreateAsync(
        ServicioCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<ServicioDto>(validationErrors.ToArray());
        }

        var referenceError = await ValidateReferencesAsync(
            dto.CategoriaId, dto.TecnicoId, cancellationToken);
        if (referenceError is not null)
        {
            return Failure<ServicioDto>(referenceError);
        }

        var codigo = NormalizeCodigo(dto.Codigo);
        if (await servicios.ExisteCodigoAsync(codigo, cancellationToken: cancellationToken))
        {
            return Failure<ServicioDto>(
                $"Ya existe un servicio con el código '{codigo}'.");
        }

        var servicio = new Servicio
        {
            Codigo = codigo,
            Nombre = dto.Nombre.Trim(),
            Precio = dto.Precio,
            CategoriaId = dto.CategoriaId,
            TecnicoId = dto.TecnicoId,
        };

        await servicios.AddAsync(servicio, cancellationToken);

        var created = await servicios.GetByIdAsync(servicio.Id, cancellationToken);
        return Success(ToDto(created!));
    }

    public async Task<ServiceResult<ServicioDto>> UpdateAsync(
        Guid id,
        ServicioUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<ServicioDto>(validationErrors.ToArray());
        }

        var servicio = await servicios.GetByIdAsync(id, cancellationToken);
        if (servicio is null)
        {
            return Failure<ServicioDto>("Servicio no encontrado.");
        }

        var referenceError = await ValidateReferencesAsync(
            dto.CategoriaId, dto.TecnicoId, cancellationToken);
        if (referenceError is not null)
        {
            return Failure<ServicioDto>(referenceError);
        }

        var codigo = NormalizeCodigo(dto.Codigo);
        if (await servicios.ExisteCodigoAsync(codigo, id, cancellationToken))
        {
            return Failure<ServicioDto>(
                $"Ya existe un servicio con el código '{codigo}'.");
        }

        servicio.Codigo = codigo;
        servicio.Nombre = dto.Nombre.Trim();
        servicio.Precio = dto.Precio;
        servicio.CategoriaId = dto.CategoriaId;
        servicio.TecnicoId = dto.TecnicoId;
        servicio.MarkUpdated();

        await servicios.UpdateAsync(servicio, cancellationToken);

        var updated = await servicios.GetByIdAsync(id, cancellationToken);
        return Success(ToDto(updated!));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await servicios.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Failure<bool>("Servicio no encontrado.");
        }

        return Success(true);
    }

    private async Task<string?> ValidateReferencesAsync(
        Guid categoriaId,
        Guid? tecnicoId,
        CancellationToken cancellationToken)
    {
        var categoria = await categorias.GetByIdAsync(categoriaId, cancellationToken);
        if (categoria is null)
        {
            return "La categoría indicada no existe.";
        }

        if (tecnicoId is not Guid id)
        {
            return null;
        }

        var tecnico = await tecnicos.GetByIdAsync(id, cancellationToken);
        if (tecnico is null)
        {
            return "El técnico indicado no existe.";
        }

        return tecnico.CategoriaId == categoriaId
            ? null
            : "El técnico debe pertenecer a la misma categoría del servicio.";
    }

    private static string NormalizeCodigo(string codigo) => codigo.Trim().ToUpperInvariant();

    private static ServicioDto ToDto(Servicio servicio) =>
        new()
        {
            Id = servicio.Id,
            Codigo = servicio.Codigo,
            Nombre = servicio.Nombre,
            Precio = servicio.Precio,
            CategoriaId = servicio.CategoriaId,
            NombreCategoria = servicio.Categoria.Nombre,
            TecnicoId = servicio.TecnicoId,
            NombreTecnico = servicio.Tecnico?.NombreCompleto,
            CreatedAtUtc = servicio.CreatedAtUtc,
            UpdatedAtUtc = servicio.UpdatedAtUtc,
        };
}
