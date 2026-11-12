using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;
using School.Infrastructure.Context;
using School.Infrastructure.Core;
using School.Infrastructure.Interfaces;

namespace School.Infrastructure.Repositories;

public sealed class InstructorRepository(SchoolContext context)
    : BaseRepository<Instructor>(context), IInstructorRepository
{
    public override async Task<IReadOnlyList<Instructor>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await Context.Instructors
            .Include(instructor => instructor.Department)
            .AsNoTracking()
            .OrderBy(instructor => instructor.LastName)
            .ThenBy(instructor => instructor.FirstName)
            .ToListAsync(cancellationToken);

    public override Task<Instructor?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        Context.Instructors
            .Include(instructor => instructor.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(instructor => instructor.Id == id, cancellationToken);

    public Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default) =>
        Context.Instructors.AnyAsync(
            instructor =>
                instructor.EmployeeNumber == employeeNumber
                && (!exceptId.HasValue || instructor.Id != exceptId.Value),
            cancellationToken);
}
