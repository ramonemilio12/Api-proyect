using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;
using School.Infrastructure.Context;
using School.Infrastructure.Core;
using School.Infrastructure.Interfaces;

namespace School.Infrastructure.Repositories;

public sealed class DepartmentRepository(SchoolContext context)
    : BaseRepository<Department>(context), IDepartmentRepository
{
    public Task<bool> CodeExistsAsync(
        string code,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default) =>
        Context.Departments.AnyAsync(
            department =>
                department.Code == code
                && (!exceptId.HasValue || department.Id != exceptId.Value),
            cancellationToken);
}
