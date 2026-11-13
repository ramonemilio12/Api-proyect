using Microsoft.AspNetCore.Mvc;
using School.Domain.Entities;
using School.Infrastructure.Exceptions;
using School.Infrastructure.Interfaces;
using School.Infrastructure.Models;

namespace School.Api.Controllers;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController(IDepartmentRepository departments)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentModel>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await departments.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentModel>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);
        return department is null ? NotFound() : Ok(ToModel(department));
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentModel>> Create(
        DepartmentWriteModel model,
        CancellationToken cancellationToken)
    {
        var department = new Department
        {
            Code = NormalizeCode(model.Code),
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
        };
        try
        {
            await departments.AddAsync(department, cancellationToken);
        }
        catch (DepartmentException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, ToModel(department));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DepartmentModel>> Update(
        Guid id,
        DepartmentWriteModel model,
        CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        department.Code = NormalizeCode(model.Code);
        department.Name = model.Name.Trim();
        department.Description = model.Description?.Trim();
        try
        {
            await departments.UpdateAsync(department, cancellationToken);
        }
        catch (DepartmentException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return Ok(ToModel(department));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await departments.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static DepartmentModel ToModel(Department department) =>
        new(
            department.Id,
            department.Code,
            department.Name,
            department.Description,
            department.CreatedAtUtc,
            department.UpdatedAtUtc);
}
