using School.Application.Contract;
using School.Application.Core;
using School.Application.Dtos.Course;
using School.Domain.Entities;
using School.Domain.Repository;
using School.Infrastructure.Interfaces;

namespace School.Application.Service;

public sealed class CourseService(
    ICourseRepository courses,
    IDepartmentRepository departments,
    IRepository<Instructor> instructors)
    : BaseService, ICourseService
{
    public async Task<ServiceResult<IReadOnlyList<CourseDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await courses.GetAllAsync(cancellationToken);
        return Success<IReadOnlyList<CourseDto>>(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<CourseDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var course = await courses.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            return Failure<CourseDto>("Curso no encontrado.");
        }

        return Success(ToDto(course));
    }

    public async Task<ServiceResult<CourseDto>> CreateAsync(
        CourseCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<CourseDto>(validationErrors.ToArray());
        }

        var referenceError = await ValidateReferencesAsync(
            dto.DepartmentId, dto.InstructorId, cancellationToken);
        if (referenceError is not null)
        {
            return Failure<CourseDto>(referenceError);
        }

        var code = NormalizeCode(dto.Code);
        if (await courses.CodeExistsAsync(code, cancellationToken: cancellationToken))
        {
            return Failure<CourseDto>($"Ya existe un curso con el código '{code}'.");
        }

        var course = new Course
        {
            Code = code,
            Name = dto.Name.Trim(),
            Credits = dto.Credits,
            DepartmentId = dto.DepartmentId,
            InstructorId = dto.InstructorId,
        };

        await courses.AddAsync(course, cancellationToken);

        var created = await courses.GetByIdAsync(course.Id, cancellationToken);
        return Success(ToDto(created!));
    }

    public async Task<ServiceResult<CourseDto>> UpdateAsync(
        Guid id,
        CourseUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<CourseDto>(validationErrors.ToArray());
        }

        var course = await courses.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            return Failure<CourseDto>("Curso no encontrado.");
        }

        var referenceError = await ValidateReferencesAsync(
            dto.DepartmentId, dto.InstructorId, cancellationToken);
        if (referenceError is not null)
        {
            return Failure<CourseDto>(referenceError);
        }

        var code = NormalizeCode(dto.Code);
        if (await courses.CodeExistsAsync(code, id, cancellationToken))
        {
            return Failure<CourseDto>($"Ya existe un curso con el código '{code}'.");
        }

        course.Code = code;
        course.Name = dto.Name.Trim();
        course.Credits = dto.Credits;
        course.DepartmentId = dto.DepartmentId;
        course.InstructorId = dto.InstructorId;
        course.MarkUpdated();

        await courses.UpdateAsync(course, cancellationToken);

        var updated = await courses.GetByIdAsync(id, cancellationToken);
        return Success(ToDto(updated!));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await courses.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Failure<bool>("Curso no encontrado.");
        }

        return Success(true);
    }

    private async Task<string?> ValidateReferencesAsync(
        Guid departmentId,
        Guid? instructorId,
        CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(departmentId, cancellationToken);
        if (department is null)
        {
            return "El departamento indicado no existe.";
        }

        if (instructorId is not Guid id)
        {
            return null;
        }

        var instructor = await instructors.GetByIdAsync(id, cancellationToken);
        if (instructor is null)
        {
            return "El instructor indicado no existe.";
        }

        return instructor.DepartmentId == departmentId
            ? null
            : "El instructor debe pertenecer al mismo departamento del curso.";
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static CourseDto ToDto(Course course) =>
        new()
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            Credits = course.Credits,
            DepartmentId = course.DepartmentId,
            DepartmentName = course.Department.Name,
            InstructorId = course.InstructorId,
            InstructorName = course.Instructor?.FullName,
            CreatedAtUtc = course.CreatedAtUtc,
            UpdatedAtUtc = course.UpdatedAtUtc,
        };
}
