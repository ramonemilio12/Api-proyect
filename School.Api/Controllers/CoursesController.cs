using Microsoft.AspNetCore.Mvc;
using School.Domain.Entities;
using School.Domain.Repository;
using School.Infrastructure.Exceptions;
using School.Infrastructure.Interfaces;
using School.Infrastructure.Models;

namespace School.Api.Controllers;

[ApiController]
[Route("api/courses")]
public sealed class CoursesController(
    ICourseRepository courses,
    IDepartmentRepository departments,
    IRepository<Instructor> instructors) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CursoModel>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await courses.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CursoModel>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var course = await courses.GetByIdAsync(id, cancellationToken);
        return course is null ? NotFound() : Ok(ToModel(course));
    }

    [HttpPost]
    public async Task<ActionResult<CursoModel>> Create(
        CourseWriteModel model,
        CancellationToken cancellationToken)
    {
        var invalidReference = await ValidateReferencesAsync(model, cancellationToken);
        if (invalidReference is not null)
        {
            return BadRequest(new { message = invalidReference });
        }

        var course = new Course();
        Apply(course, model);
        try
        {
            await courses.AddAsync(course, cancellationToken);
        }
        catch (CourseException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var created = await courses.GetByIdAsync(course.Id, cancellationToken);
        return CreatedAtAction(
            nameof(GetById), new { id = course.Id }, ToModel(created!));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CursoModel>> Update(
        Guid id,
        CourseWriteModel model,
        CancellationToken cancellationToken)
    {
        var course = await courses.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            return NotFound();
        }

        var invalidReference = await ValidateReferencesAsync(model, cancellationToken);
        if (invalidReference is not null)
        {
            return BadRequest(new { message = invalidReference });
        }

        Apply(course, model);
        try
        {
            await courses.UpdateAsync(course, cancellationToken);
        }
        catch (CourseException exception)
        {
            return Conflict(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var updated = await courses.GetByIdAsync(id, cancellationToken);
        return Ok(ToModel(updated!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await courses.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private async Task<string?> ValidateReferencesAsync(
        CourseWriteModel model,
        CancellationToken cancellationToken)
    {
        var department = await departments.GetByIdAsync(
            model.DepartmentId, cancellationToken);
        if (department is null)
        {
            return "El departamento indicado no existe.";
        }

        if (model.InstructorId is not Guid instructorId)
        {
            return null;
        }

        var instructor = await instructors.GetByIdAsync(
            instructorId, cancellationToken);
        if (instructor is null)
        {
            return "El instructor indicado no existe.";
        }

        return instructor.DepartmentId == model.DepartmentId
            ? null
            : "El instructor debe pertenecer al mismo departamento del curso.";
    }

    private static void Apply(Course course, CourseWriteModel model)
    {
        course.Code = NormalizeCode(model.Code);
        course.Name = model.Name.Trim();
        course.Credits = model.Credits;
        course.DepartmentId = model.DepartmentId;
        course.InstructorId = model.InstructorId;
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static CursoModel ToModel(Course course) =>
        new(
            course.Id,
            course.Code,
            course.Name,
            course.Credits,
            course.DepartmentId,
            course.Department.Name,
            course.InstructorId,
            course.Instructor?.FullName,
            course.CreatedAtUtc,
            course.UpdatedAtUtc);
}
