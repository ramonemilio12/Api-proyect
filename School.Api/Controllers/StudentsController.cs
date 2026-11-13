using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using School.Domain.Entities;
using School.Domain.Repository;
using School.Infrastructure.Interfaces;

namespace School.Api.Controllers;

[ApiController]
[Route("api/students")]
public sealed class StudentsController(
    IRepository<Student> students,
    IDepartmentRepository departments) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Student>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await students.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Student>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(id, cancellationToken);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<Student>> Create(
        StudentRequest model,
        CancellationToken cancellationToken)
    {
        if (await departments.GetByIdAsync(model.DepartmentId, cancellationToken) is null)
        {
            return BadRequest(new { message = "El departamento indicado no existe." });
        }

        var student = new Student();
        Apply(student, model);
        try
        {
            await students.AddAsync(student, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Student>> Update(
        Guid id,
        StudentRequest model,
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

        Apply(student, model);
        try
        {
            await students.UpdateAsync(student, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return Ok(student);
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
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static void Apply(Student student, StudentRequest model)
    {
        student.StudentNumber = model.StudentNumber.Trim().ToUpperInvariant();
        student.FirstName = model.FirstName.Trim();
        student.LastName = model.LastName.Trim();
        student.Email = model.Email.Trim().ToLowerInvariant();
        student.PhoneNumber = model.PhoneNumber?.Trim();
        student.DepartmentId = model.DepartmentId;
    }
}

public sealed class StudentRequest
{
    [Required, StringLength(30, MinimumLength = 2)]
    public string StudentNumber { get; init; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 1)]
    public string FirstName { get; init; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 1)]
    public string LastName { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; init; } = string.Empty;

    [Phone, StringLength(30)]
    public string? PhoneNumber { get; init; }

    public Guid DepartmentId { get; init; }
}
