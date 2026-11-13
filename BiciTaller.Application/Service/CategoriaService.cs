using BiciTaller.Application.Contract;
using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Categoria;
using BiciTaller.Domain.Entities;
using BiciTaller.Infrastructure.Interfaces;

namespace BiciTaller.Application.Service;

public sealed class CategoriaService(ICategoriaRepository repository)
    : BaseService, ICategoriaService
{
    public async Task<ServiceResult<IReadOnlyList<CategoriaDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var categorias = await repository.GetAllAsync(cancellationToken);
        var dtos = categorias.Select(ToDto).ToList();
        return Success<IReadOnlyList<CategoriaDto>>(dtos);
    }

    public async Task<ServiceResult<CategoriaDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var categoria = await repository.GetByIdAsync(id, cancellationToken);
        if (categoria is null)
        {
            return Failure<CategoriaDto>("Categoría no encontrada.");
        }

        return Success(ToDto(categoria));
    }

    public async Task<ServiceResult<CategoriaDto>> CreateAsync(
        CategoriaCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<CategoriaDto>(validationErrors.ToArray());
        }

        var codigo = NormalizeCodigo(dto.Codigo);
        if (await repository.ExisteCodigoAsync(codigo, cancellationToken: cancellationToken))
        {
            return Failure<CategoriaDto>(
                $"Ya existe una categoría con el código '{codigo}'.");
        }

        var categoria = new Categoria
        {
            Codigo = codigo,
            Nombre = dto.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion)
                ? null
                : dto.Descripcion.Trim(),
        };

        await repository.AddAsync(categoria, cancellationToken);
        return Success(ToDto(categoria));
    }

    public async Task<ServiceResult<CategoriaDto>> UpdateAsync(
        Guid id,
        CategoriaUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<CategoriaDto>(validationErrors.ToArray());
        }

        var categoria = await repository.GetByIdAsync(id, cancellationToken);
        if (categoria is null)
        {
            return Failure<CategoriaDto>("Categoría no encontrada.");
        }

        var codigo = NormalizeCodigo(dto.Codigo);
        if (await repository.ExisteCodigoAsync(codigo, id, cancellationToken))
        {
            return Failure<CategoriaDto>(
                $"Ya existe una categoría con el código '{codigo}'.");
        }

        categoria.Codigo = codigo;
        categoria.Nombre = dto.Nombre.Trim();
        categoria.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion)
            ? null
            : dto.Descripcion.Trim();
        categoria.MarkUpdated();

        await repository.UpdateAsync(categoria, cancellationToken);
        return Success(ToDto(categoria));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Failure<bool>("Categoría no encontrada.");
        }

        return Success(true);
    }

    private static string NormalizeCodigo(string codigo) => codigo.Trim().ToUpperInvariant();

    private static CategoriaDto ToDto(Categoria categoria) =>
        new()
        {
            Id = categoria.Id,
            Codigo = categoria.Codigo,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            CreatedAtUtc = categoria.CreatedAtUtc,
            UpdatedAtUtc = categoria.UpdatedAtUtc,
        };
}
