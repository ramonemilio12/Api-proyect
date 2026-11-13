using Microsoft.EntityFrameworkCore;
using BiciTaller.Domain.Entities;
using BiciTaller.Infrastructure.Context;
using BiciTaller.Infrastructure.Core;
using BiciTaller.Infrastructure.Exceptions;
using BiciTaller.Infrastructure.Interfaces;

namespace BiciTaller.Infrastructure.Repositories;

public sealed class ServicioRepository(BiciTallerContext context)
    : BaseRepository<Servicio>(context), IServicioRepository
{
    public override async Task<IReadOnlyList<Servicio>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await Context.Servicios
            .Include(servicio => servicio.Categoria)
            .Include(servicio => servicio.Tecnico)
            .AsNoTracking()
            .OrderBy(servicio => servicio.Codigo)
            .ToListAsync(cancellationToken);

    public override Task<Servicio?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        Context.Servicios
            .Include(servicio => servicio.Categoria)
            .Include(servicio => servicio.Tecnico)
            .AsNoTracking()
            .FirstOrDefaultAsync(servicio => servicio.Id == id, cancellationToken);

    public override async Task<Servicio> AddAsync(
        Servicio entity,
        CancellationToken cancellationToken = default)
    {
        await EnsureUniqueCodigoAsync(entity.Codigo, null, cancellationToken);
        return await base.AddAsync(entity, cancellationToken);
    }

    public override async Task<Servicio> UpdateAsync(
        Servicio entity,
        CancellationToken cancellationToken = default)
    {
        await EnsureUniqueCodigoAsync(entity.Codigo, entity.Id, cancellationToken);
        return await base.UpdateAsync(entity, cancellationToken);
    }

    public Task<bool> ExisteCodigoAsync(
        string codigo,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default) =>
        Context.Servicios.AnyAsync(
            servicio =>
                servicio.Codigo == codigo
                && (!exceptId.HasValue || servicio.Id != exceptId.Value),
            cancellationToken);

    private async Task EnsureUniqueCodigoAsync(
        string codigo,
        Guid? exceptId,
        CancellationToken cancellationToken)
    {
        if (await ExisteCodigoAsync(codigo, exceptId, cancellationToken))
        {
            throw new ServicioException($"Ya existe un servicio con el código '{codigo}'.");
        }
    }
}
