using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using BiciTaller.Domain.Core;
using BiciTaller.Domain.Repository;
using BiciTaller.Infrastructure.Context;

namespace BiciTaller.Infrastructure.Core;

public abstract class BaseRepository<TEntity>(BiciTallerContext context)
    : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected BiciTallerContext Context { get; } = context;
    protected DbSet<TEntity> Entities => Context.Set<TEntity>();

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await Entities.AsNoTracking().ToListAsync(cancellationToken);

    public virtual Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        Entities.AsNoTracking().FirstOrDefaultAsync(
            entity => entity.Id == id, cancellationToken);

    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await Entities.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        entity.MarkUpdated();
        Context.Entry(entity).State = EntityState.Modified;
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Entities.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return false;
        }

        Entities.Remove(entity);
        await SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        Entities.AsNoTracking().AnyAsync(predicate, cancellationToken);

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException(
                "La operación viola una relación o una restricción de datos.",
                exception);
        }
    }
}
