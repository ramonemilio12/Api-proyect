using Microsoft.EntityFrameworkCore;
using School.Domain.Core;
using School.Domain.Repository;
using School.Infrastructure.Context;
using School.Infrastructure.Exceptions;

namespace School.Infrastructure.Core;

public abstract class BaseRepository<TEntity>(SchoolContext context)
    : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected SchoolContext Context { get; } = context;
    protected DbSet<TEntity> Entities => Context.Set<TEntity>();

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await Entities.AsNoTracking().ToListAsync(cancellationToken);

    public virtual Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        Entities.AsNoTracking().FirstOrDefaultAsync(
            entity => entity.Id == id, cancellationToken);

    public async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await Entities.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<TEntity> UpdateAsync(
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

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            throw new DataIntegrityException(
                "La operación viola una relación o una restricción de datos.",
                exception);
        }
    }
}
