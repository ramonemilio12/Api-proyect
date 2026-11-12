using School.Domain.Entities;
using School.Domain.Repository;

namespace School.Infrastructure.Interfaces;

public interface IInstructorRepository : IRepository<Instructor>
{
    Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default);
}
