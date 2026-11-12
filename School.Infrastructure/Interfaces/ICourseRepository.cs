using School.Domain.Entities;
using School.Domain.Repository;

namespace School.Infrastructure.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<bool> CodeExistsAsync(
        string code,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default);
}
