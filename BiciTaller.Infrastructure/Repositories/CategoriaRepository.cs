using Microsoft.EntityFrameworkCore;
using BiciTaller.Domain.Entities;
using BiciTaller.Infrastructure.Context;
using BiciTaller.Infrastructure.Core;
using BiciTaller.Infrastructure.Exceptions;
using BiciTaller.Infrastructure.Interfaces;

namespace BiciTaller.Infrastructure.Repositories;

public sealed class CategoriaRepository(BiciTallerContext context)
    : BaseRepository<Categoria>(context), ICategoriaRepository
{
    public override async Task<Categoria> AddAsync(
        Categoria entity,
        CancellationToken cancellationToken = default)
    {
        await EnsureUniqueCodigoAsync(entity.Codigo, null, cancellationToken);
        return await base.AddAsync(entity, cancellationToken);
    }

    public override async Task<Categoria> UpdateAsync(
        Categoria entity,
        CancellationToken cancellationToken = default)
    {
        await EnsureUniqueCodigoAsync(entity.Codigo, entity.Id, cancellationToken);
        return await base.UpdateAsync(entity, cancellationToken);
    }

    public Task<bool> ExisteCodigoAsync(
        string codigo,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default) =>
        Context.Categorias.AnyAsync(
            categoria =>
                categoria.Codigo == codigo
                && (!exceptId.HasValue || categoria.Id != exceptId.Value),
            cancellationToken);

    private async Task EnsureUniqueCodigoAsync(
        string codigo,
        Guid? exceptId,
        CancellationToken cancellationToken)
    {
        if (await ExisteCodigoAsync(codigo, exceptId, cancellationToken))
        {
            throw new CategoriaException(
                $"Ya existe una categoría con el código '{codigo}'.");
        }
    }
}
