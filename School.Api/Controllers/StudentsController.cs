using Microsoft.AspNetCore.Mvc;
using School.Domain.Entities;
using School.Infrastructure.Exceptions;
using School.Infrastructure.Interfaces;
using School.Infrastructure.Models;

namespace School.Api.Controllers;

[ApiController]
[Route("api/students")]
public sealed class StudentsController(
    IStudentRepository students,
    IDepartmentRepository departments) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StudentModel>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await students.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentModel>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(id, cancellationToken);
        return student is null ? NotFound() : Ok(ToModel(student));
    }

    [HttpPost]
    public async Task<ActionResult<StudentModel>> Create(
        StudentWriteModel model,
        CancellationToken cancellationToken)
    {
        if (await departments.GetByIdAsync(model.DepartmentId, cancellationToken) is null)
        {
            return BadRequest(new { message = "El departamento indicado no existe." });
        }

        var studentNumber = model.StudentNumber.Trim().ToUpperInvariant();
        if (await students.StudentNumberExistsAsync(
                studentNumber, cancellationToken: cancellationToken))
        {
            return Conflict(new { message = "Ya existe un estudiante con ese código." });
        }

        var student = new Student();
        Apply(student, model, studentNumber);
        try
        {
            await students.AddAsync(student, cancellationToken);
        }
        catch (DataIntegrityException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var created = await students.GetByIdAsync(student.Id, cancellationToken);
        return CreatedAtAction(
            nameof(GetById), new { id = student.Id }, ToModel(created!));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StudentModel>> Update(
        Guid id,
        StudentWriteModel model,
        CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return NotFound();
        }

        if (await departments.GetByIdAsync(model.DepartmentId, cancellationToken) is null)
        {
            return BadRequest(new { message = "El departamento indicado no existe." });
        }

        var studentNumber = model.StudentNumber.Trim().ToUpperInvariant();
        if (await students.StudentNumberExistsAsync(
                studentNumber, id, cancellationToken))
        {
            return Conflict(new { message = "Ya existe un estudiante con ese código." });
        }

        Apply(student, model, studentNumber);
        try
        {
            await students.UpdateAsync(student, cancellationToken);
        }
        catch (DataIntegrityException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var updated = await students.GetByIdAsync(id, cancellationToken);
        return Ok(ToModel(updated!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await students.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (DataIntegrityException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static void Apply(
        Student student,
        StudentWriteModel model,
        string studentNumber)
    {
        student.StudentNumber = studentNumber;
        student.FirstName = model.FirstName.Trim();
        student.LastName = model.LastName.Trim();
        student.Email = model.Email.Trim().ToLowerInvariant();
        student.PhoneNumber = model.PhoneNumber?.Trim();
        student.DepartmentId = model.DepartmentId;
    }

    private static StudentModel ToModel(Student student) =>
        new(
            student.Id,
            student.StudentNumber,
            student.FirstName,
            student.LastName,
            student.FullName,
            student.Email,
            student.PhoneNumber,
            student.DepartmentId,
            student.Department.Name,
            student.EnrollmentDateUtc,
            student.CreatedAtUtc,
            student.UpdatedAtUtc);
}
