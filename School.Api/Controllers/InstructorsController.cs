using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using School.Domain.Entities;
using School.Domain.Repository;
using School.Infrastructure.Interfaces;

namespace School.Api.Controllers;

[ApiController]
[Route("api/instructors")]
public sealed class InstructorsController(
    IRepository<Instructor> instructors,
    IDepartmentRepository departments) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Instructor>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await instructors.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Instructor>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var instructor = await instructors.GetByIdAsync(id, cancellationToken);
        return instructor is null ? NotFound() : Ok(instructor);
    }

    [HttpPost]
    public async Task<ActionResult<Instructor>> Create(
        InstructorRequest model,
        CancellationToken cancellationToken)
    {
        if (await departments.GetByIdAsync(model.DepartmentId, cancellationToken) is null)
        {
            return BadRequest(new { message = "El departamento indicado no existe." });
        }

        var instructor = new Instructor();
        Apply(instructor, model);
        try
        {
            await instructors.AddAsync(instructor, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = instructor.Id }, instructor);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Instructor>> Update(
        Guid id,
        InstructorRequest model,
        CancellationToken cancellationToken)
    {
        var instructor = await instructors.GetByIdAsync(id, cancellationToken);
        if (instructor is null)
        {
            return NotFound();
        }

        if (await departments.GetByIdAsync(model.DepartmentId, cancellationToken) is null)
        {
            return BadRequest(new { message = "El departamento indicado no existe." });
        }

        Apply(instructor, model);
        try
        {
            await instructors.UpdateAsync(instructor, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        return Ok(instructor);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            return await instructors.DeleteAsync(id, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static void Apply(Instructor instructor, InstructorRequest model)
    {
        instructor.EmployeeNumber = model.EmployeeNumber.Trim().ToUpperInvariant();
        instructor.FirstName = model.FirstName.Trim();
        instructor.LastName = model.LastName.Trim();
        instructor.Email = model.Email.Trim().ToLowerInvariant();
        instructor.PhoneNumber = model.PhoneNumber?.Trim();
        instructor.DepartmentId = model.DepartmentId;
    }
}

public sealed class InstructorRequest
{
    [Required, StringLength(30, MinimumLength = 2)]
    public string EmployeeNumber { get; init; } = string.Empty;

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
