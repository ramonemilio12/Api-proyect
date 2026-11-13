using Microsoft.AspNetCore.Mvc;
using School.Application.Contract;
using School.Application.Core;
using School.Application.Dtos.Department;

namespace School.Api.Controllers;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController(IDepartamentService service)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Map(result);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create(
        DepartmentCreateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(dto, cancellationToken);
        if (!result.Success)
        {
            return Conflict(new { errors = result.Errors });
        }

        return CreatedAtAction(
            nameof(GetById), new { id = result.Data.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DepartmentDto>> Update(
        Guid id,
        DepartmentUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, dto, cancellationToken);
        return Map(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.Success ? NoContent() : NotFound(new { errors = result.Errors });
    }

    private ActionResult<DepartmentDto> Map(ServiceResult<DepartmentDto> result)
    {
        if (!result.Success)
        {
            return NotFound(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }
}
