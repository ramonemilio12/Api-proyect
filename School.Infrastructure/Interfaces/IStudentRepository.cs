using School.Domain.Entities;
using School.Domain.Repository;

namespace School.Infrastructure.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<bool> StudentNumberExistsAsync(
        string studentNumber,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default);
}
