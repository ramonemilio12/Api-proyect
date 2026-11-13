using BiciTaller.Domain.Core;
using BiciTaller.Infrastructure.Context;

namespace BiciTaller.Infrastructure.Core;

public sealed class Repository<TEntity>(BiciTallerContext context)
    : BaseRepository<TEntity>(context)
    where TEntity : BaseEntity
{
}
