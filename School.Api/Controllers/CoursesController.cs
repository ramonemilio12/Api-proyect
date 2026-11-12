using Microsoft.AspNetCore.Mvc;
using School.Domain.Entities;
using School.Infrastructure.Exceptions;
using School.Infrastructure.Interfaces;
using School.Infrastructure.Models;

namespace School.Api.Controllers;

[ApiController]
[Route("api/courses")]
public sealed class CoursesController(
    ICourseRepository courses,
    IDepartmentRepository departments,
    IInstructorRepository instructors) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CourseModel>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await courses.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CourseModel>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var course = await courses.GetByIdAsync(id, cancellationToken);
        return course is null ? NotFound() : Ok(ToModel(course));
    }

    [HttpPost]
    public async Task<ActionResult<CourseModel>> Create(
        CourseWriteModel model,
        CancellationToken cancellationToken)
    {
        var invalidReference = await ValidateReferencesAsync(model, cancellationToken);
        if (invalidReference is not null)
        {
            return BadRequest(new { message = invalidReference });
        }

        var code = NormalizeCode(model.Code);
        if (await courses.CodeExistsAsync(code, cancellationToken: cancellationToken))
        {
            return Conflict(new { message = "Ya existe un curso con ese código." });
        }

        var course = new Course();
        Apply(course, model, code);
        try
        {
            await courses.AddAsync(course, cancellationToken);
        }
        catch (DataIntegrityException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var created = await courses.GetByIdAsync(course.Id, cancellationToken);
        return CreatedAtAction(
            nameof(GetById), new { id = course.Id }, ToModel(created!));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CourseModel>> Update(
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

        var code = NormalizeCode(model.Code);
        if (await courses.CodeExistsAsync(code, id, cancellationToken))
        {
            return Conflict(new { message = "Ya existe un curso con ese código." });
        }

        Apply(course, model, code);
        try
        {
            await courses.UpdateAsync(course, cancellationToken);
        }
        catch (DataIntegrityException exception)
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
        catch (DataIntegrityException exception)
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

    private static void Apply(Course course, CourseWriteModel model, string code)
    {
        course.Code = code;
        course.Name = model.Name.Trim();
        course.Credits = model.Credits;
        course.DepartmentId = model.DepartmentId;
        course.InstructorId = model.InstructorId;
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static CourseModel ToModel(Course course) =>
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
