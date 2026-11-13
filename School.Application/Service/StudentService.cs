using School.Application.Contract;
using School.Application.Core;
using School.Application.Dtos.Student;
using School.Domain.Entities;
using School.Domain.Repository;
using School.Infrastructure.Interfaces;

namespace School.Application.Service;

public sealed class StudentService(
    IRepository<Student> students,
    IDepartmentRepository departments)
    : BaseService, IStudentService
{
    public async Task<ServiceResult<IReadOnlyList<StudentDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await students.GetAllAsync(cancellationToken);
        return Success<IReadOnlyList<StudentDto>>(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<StudentDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var student = await students.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return Failure<StudentDto>("Estudiante no encontrado.");
        }

        return Success(ToDto(student));
    }

    public async Task<ServiceResult<StudentDto>> CreateAsync(
        StudentCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<StudentDto>(validationErrors.ToArray());
        }

        var department = await departments.GetByIdAsync(dto.DepartmentId, cancellationToken);
        if (department is null)
        {
            return Failure<StudentDto>("El departamento indicado no existe.");
        }

        var studentNumber = Normalize(dto.StudentNumber);
        if (await students.ExistsAsync(
            s => s.StudentNumber == studentNumber, cancellationToken))
        {
            return Failure<StudentDto>(
                $"Ya existe un estudiante con el número '{studentNumber}'.");
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await students.ExistsAsync(s => s.Email == email, cancellationToken))
        {
            return Failure<StudentDto>($"Ya existe un estudiante con el correo '{email}'.");
        }

        var student = new Student
        {
            StudentNumber = studentNumber,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = email,
            PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
                ? null
                : dto.PhoneNumber.Trim(),
            DepartmentId = dto.DepartmentId,
        };

        await students.AddAsync(student, cancellationToken);
        return Success(ToDto(student));
    }

    public async Task<ServiceResult<StudentDto>> UpdateAsync(
        Guid id,
        StudentUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<StudentDto>(validationErrors.ToArray());
        }

        var student = await students.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return Failure<StudentDto>("Estudiante no encontrado.");
        }

        var department = await departments.GetByIdAsync(dto.DepartmentId, cancellationToken);
        if (department is null)
        {
            return Failure<StudentDto>("El departamento indicado no existe.");
        }

        var studentNumber = Normalize(dto.StudentNumber);
        if (await students.ExistsAsync(
            s => s.StudentNumber == studentNumber && s.Id != id, cancellationToken))
        {
            return Failure<StudentDto>(
                $"Ya existe un estudiante con el número '{studentNumber}'.");
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await students.ExistsAsync(
            s => s.Email == email && s.Id != id, cancellationToken))
        {
            return Failure<StudentDto>($"Ya existe un estudiante con el correo '{email}'.");
        }

        student.StudentNumber = studentNumber;
        student.FirstName = dto.FirstName.Trim();
        student.LastName = dto.LastName.Trim();
        student.Email = email;
        student.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
            ? null
            : dto.PhoneNumber.Trim();
        student.DepartmentId = dto.DepartmentId;
        student.MarkUpdated();

        await students.UpdateAsync(student, cancellationToken);
        return Success(ToDto(student));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await students.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Failure<bool>("Estudiante no encontrado.");
        }

        return Success(true);
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();

    private static StudentDto ToDto(Student student) =>
        new()
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            FirstName = student.FirstName,
            LastName = student.LastName,
            FullName = student.FullName,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            DepartmentId = student.DepartmentId,
            DepartmentName = student.Department?.Name ?? string.Empty,
            EnrollmentDateUtc = student.EnrollmentDateUtc,
            CreatedAtUtc = student.CreatedAtUtc,
            UpdatedAtUtc = student.UpdatedAtUtc,
        };
}
