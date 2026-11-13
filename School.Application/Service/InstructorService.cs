using School.Application.Contract;
using School.Application.Core;
using School.Application.Dtos.Instructor;
using School.Domain.Entities;
using School.Domain.Repository;
using School.Infrastructure.Interfaces;

namespace School.Application.Service;

public sealed class InstructorService(
    IRepository<Instructor> instructors,
    IDepartmentRepository departments)
    : BaseService, IInstructorService
{
    public async Task<ServiceResult<IReadOnlyList<InstructorDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await instructors.GetAllAsync(cancellationToken);
        return Success<IReadOnlyList<InstructorDto>>(items.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<InstructorDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var instructor = await instructors.GetByIdAsync(id, cancellationToken);
        if (instructor is null)
        {
            return Failure<InstructorDto>("Instructor no encontrado.");
        }

        return Success(ToDto(instructor));
    }

    public async Task<ServiceResult<InstructorDto>> CreateAsync(
        InstructorCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<InstructorDto>(validationErrors.ToArray());
        }

        var department = await departments.GetByIdAsync(dto.DepartmentId, cancellationToken);
        if (department is null)
        {
            return Failure<InstructorDto>("El departamento indicado no existe.");
        }

        var employeeNumber = Normalize(dto.EmployeeNumber);
        if (await instructors.ExistsAsync(
            i => i.EmployeeNumber == employeeNumber, cancellationToken))
        {
            return Failure<InstructorDto>(
                $"Ya existe un instructor con el número '{employeeNumber}'.");
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await instructors.ExistsAsync(i => i.Email == email, cancellationToken))
        {
            return Failure<InstructorDto>(
                $"Ya existe un instructor con el correo '{email}'.");
        }

        var instructor = new Instructor
        {
            EmployeeNumber = employeeNumber,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = email,
            PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
                ? null
                : dto.PhoneNumber.Trim(),
            DepartmentId = dto.DepartmentId,
        };

        await instructors.AddAsync(instructor, cancellationToken);
        return Success(ToDto(instructor));
    }

    public async Task<ServiceResult<InstructorDto>> UpdateAsync(
        Guid id,
        InstructorUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<InstructorDto>(validationErrors.ToArray());
        }

        var instructor = await instructors.GetByIdAsync(id, cancellationToken);
        if (instructor is null)
        {
            return Failure<InstructorDto>("Instructor no encontrado.");
        }

        var department = await departments.GetByIdAsync(dto.DepartmentId, cancellationToken);
        if (department is null)
        {
            return Failure<InstructorDto>("El departamento indicado no existe.");
        }

        var employeeNumber = Normalize(dto.EmployeeNumber);
        if (await instructors.ExistsAsync(
            i => i.EmployeeNumber == employeeNumber && i.Id != id, cancellationToken))
        {
            return Failure<InstructorDto>(
                $"Ya existe un instructor con el número '{employeeNumber}'.");
        }

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await instructors.ExistsAsync(
            i => i.Email == email && i.Id != id, cancellationToken))
        {
            return Failure<InstructorDto>(
                $"Ya existe un instructor con el correo '{email}'.");
        }

        instructor.EmployeeNumber = employeeNumber;
        instructor.FirstName = dto.FirstName.Trim();
        instructor.LastName = dto.LastName.Trim();
        instructor.Email = email;
        instructor.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
            ? null
            : dto.PhoneNumber.Trim();
        instructor.DepartmentId = dto.DepartmentId;
        instructor.MarkUpdated();

        await instructors.UpdateAsync(instructor, cancellationToken);
        return Success(ToDto(instructor));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await instructors.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Failure<bool>("Instructor no encontrado.");
        }

        return Success(true);
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();

    private static InstructorDto ToDto(Instructor instructor) =>
        new()
        {
            Id = instructor.Id,
            EmployeeNumber = instructor.EmployeeNumber,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            FullName = instructor.FullName,
            Email = instructor.Email,
            PhoneNumber = instructor.PhoneNumber,
            DepartmentId = instructor.DepartmentId,
            DepartmentName = instructor.Department?.Name ?? string.Empty,
            HireDateUtc = instructor.HireDateUtc,
            CreatedAtUtc = instructor.CreatedAtUtc,
            UpdatedAtUtc = instructor.UpdatedAtUtc,
        };
}
