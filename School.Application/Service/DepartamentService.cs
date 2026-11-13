using School.Application.Contract;
using School.Application.Core;
using School.Application.Dtos.Department;
using School.Domain.Entities;
using School.Infrastructure.Interfaces;

namespace School.Application.Service;

public sealed class DepartamentService(IDepartmentRepository repository)
    : BaseService, IDepartamentService
{
    public async Task<ServiceResult<IReadOnlyList<DepartmentDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var departments = await repository.GetAllAsync(cancellationToken);
        var dtos = departments.Select(ToDto).ToList();
        return Success<IReadOnlyList<DepartmentDto>>(dtos);
    }

    public async Task<ServiceResult<DepartmentDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var department = await repository.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return Failure<DepartmentDto>("Departamento no encontrado.");
        }

        return Success(ToDto(department));
    }

    public async Task<ServiceResult<DepartmentDto>> CreateAsync(
        DepartmentCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<DepartmentDto>(validationErrors.ToArray());
        }

        var code = NormalizeCode(dto.Code);
        if (await repository.CodeExistsAsync(code, cancellationToken: cancellationToken))
        {
            return Failure<DepartmentDto>(
                $"Ya existe un departamento con el código '{code}'.");
        }

        var department = new Department
        {
            Code = code,
            Name = dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim(),
        };

        await repository.AddAsync(department, cancellationToken);
        return Success(ToDto(department));
    }

    public async Task<ServiceResult<DepartmentDto>> UpdateAsync(
        Guid id,
        DepartmentUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!TryValidate(dto, out var validationErrors))
        {
            return Failure<DepartmentDto>(validationErrors.ToArray());
        }

        var department = await repository.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return Failure<DepartmentDto>("Departamento no encontrado.");
        }

        var code = NormalizeCode(dto.Code);
        if (await repository.CodeExistsAsync(code, id, cancellationToken))
        {
            return Failure<DepartmentDto>(
                $"Ya existe un departamento con el código '{code}'.");
        }

        department.Code = code;
        department.Name = dto.Name.Trim();
        department.Description = string.IsNullOrWhiteSpace(dto.Description)
            ? null
            : dto.Description.Trim();
        department.MarkUpdated();

        await repository.UpdateAsync(department, cancellationToken);
        return Success(ToDto(department));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return Failure<bool>("Departamento no encontrado.");
        }

        return Success(true);
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static DepartmentDto ToDto(Department department) =>
        new()
        {
            Id = department.Id,
            Code = department.Code,
            Name = department.Name,
            Description = department.Description,
            CreatedAtUtc = department.CreatedAtUtc,
            UpdatedAtUtc = department.UpdatedAtUtc,
        };
}
