using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;
using School.Infrastructure.Context;
using School.Infrastructure.Core;
using School.Infrastructure.Interfaces;

namespace School.Infrastructure.Repositories;

public sealed class CourseRepository(SchoolContext context)
    : BaseRepository<Course>(context), ICourseRepository
{
    public override async Task<IReadOnlyList<Course>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await Context.Courses
            .Include(course => course.Department)
            .Include(course => course.Instructor)
            .AsNoTracking()
            .OrderBy(course => course.Code)
            .ToListAsync(cancellationToken);

    public override Task<Course?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        Context.Courses
            .Include(course => course.Department)
            .Include(course => course.Instructor)
            .AsNoTracking()
            .FirstOrDefaultAsync(course => course.Id == id, cancellationToken);

    public Task<bool> CodeExistsAsync(
        string code,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default) =>
        Context.Courses.AnyAsync(
            course =>
                course.Code == code
                && (!exceptId.HasValue || course.Id != exceptId.Value),
            cancellationToken);
}
