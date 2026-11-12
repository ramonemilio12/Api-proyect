using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;
using School.Infrastructure.Context;
using School.Infrastructure.Core;
using School.Infrastructure.Interfaces;

namespace School.Infrastructure.Repositories;

public sealed class StudentRepository(SchoolContext context)
    : BaseRepository<Student>(context), IStudentRepository
{
    public override async Task<IReadOnlyList<Student>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await Context.Students
            .Include(student => student.Department)
            .AsNoTracking()
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ToListAsync(cancellationToken);

    public override Task<Student?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        Context.Students
            .Include(student => student.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == id, cancellationToken);

    public Task<bool> StudentNumberExistsAsync(
        string studentNumber,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default) =>
        Context.Students.AnyAsync(
            student =>
                student.StudentNumber == studentNumber
                && (!exceptId.HasValue || student.Id != exceptId.Value),
            cancellationToken);
}
