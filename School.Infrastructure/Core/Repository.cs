using School.Domain.Core;
using School.Infrastructure.Context;

namespace School.Infrastructure.Core;

public sealed class Repository<TEntity>(SchoolContext context)
    : BaseRepository<TEntity>(context)
    where TEntity : BaseEntity
{
}
