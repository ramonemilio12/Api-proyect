using School.Domain.Entities;
using School.Domain.Repository;

namespace School.Infrastructure.Interfaces;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<bool> CodeExistsAsync(
        string code,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default);
}
