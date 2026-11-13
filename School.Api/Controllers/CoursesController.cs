using Microsoft.AspNetCore.Mvc;
using School.Application.Contract;
using School.Application.Core;
using School.Application.Dtos.Course;

namespace School.Api.Controllers;

[ApiController]
[Route("api/courses")]
public sealed class CoursesController(ICourseService service)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CourseDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Map(result);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create(
        CourseCreateDto dto,
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
    public async Task<ActionResult<CourseDto>> Update(
        Guid id,
        CourseUpdateDto dto,
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

    private ActionResult<CourseDto> Map(ServiceResult<CourseDto> result)
    {
        if (!result.Success)
        {
            return NotFound(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }
}
