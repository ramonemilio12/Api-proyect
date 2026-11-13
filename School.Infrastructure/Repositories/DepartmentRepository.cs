using Microsoft.EntityFrameworkCore;
using School.Domain.Entities;
using School.Infrastructure.Context;
using School.Infrastructure.Core;
using School.Infrastructure.Exceptions;
using School.Infrastructure.Interfaces;

namespace School.Infrastructure.Repositories;

public sealed class DepartmentRepository(SchoolContext context)
    : BaseRepository<Department>(context), IDepartmentRepository
{
    public override async Task<Department> AddAsync(
        Department entity,
        CancellationToken cancellationToken = default)
    {
        await EnsureUniqueCodeAsync(entity.Code, null, cancellationToken);
        return await base.AddAsync(entity, cancellationToken);
    }

    public override async Task<Department> UpdateAsync(
        Department entity,
        CancellationToken cancellationToken = default)
    {
        await EnsureUniqueCodeAsync(entity.Code, entity.Id, cancellationToken);
        return await base.UpdateAsync(entity, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(
        string code,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default) =>
        Context.Departments.AnyAsync(
            department =>
                department.Code == code
                && (!exceptId.HasValue || department.Id != exceptId.Value),
            cancellationToken);

    private async Task EnsureUniqueCodeAsync(
        string code,
        Guid? exceptId,
        CancellationToken cancellationToken)
    {
        if (await CodeExistsAsync(code, exceptId, cancellationToken))
        {
            throw new DepartmentException(
                $"Ya existe un departamento con el código '{code}'.");
        }
    }
}
