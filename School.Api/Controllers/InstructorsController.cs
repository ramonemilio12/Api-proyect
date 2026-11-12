using Microsoft.AspNetCore.Mvc;
using School.Domain.Entities;
using School.Infrastructure.Exceptions;
using School.Infrastructure.Interfaces;
using School.Infrastructure.Models;

namespace School.Api.Controllers;

[ApiController]
[Route("api/instructors")]
public sealed class InstructorsController(
    IInstructorRepository instructors,
    IDepartmentRepository departments) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InstructorModel>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await instructors.GetAllAsync(cancellationToken);
        return Ok(items.Select(ToModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InstructorModel>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var instructor = await instructors.GetByIdAsync(id, cancellationToken);
        return instructor is null ? NotFound() : Ok(ToModel(instructor));
    }

    [HttpPost]
    public async Task<ActionResult<InstructorModel>> Create(
        InstructorWriteModel model,
        CancellationToken cancellationToken)
    {
        if (await departments.GetByIdAsync(model.DepartmentId, cancellationToken) is null)
        {
            return BadRequest(new { message = "El departamento indicado no existe." });
        }

        var employeeNumber = model.EmployeeNumber.Trim().ToUpperInvariant();
        if (await instructors.EmployeeNumberExistsAsync(
                employeeNumber, cancellationToken: cancellationToken))
        {
            return Conflict(new { message = "Ya existe un instructor con ese código." });
        }

        var instructor = new Instructor();
        Apply(instructor, model, employeeNumber);
        try
        {
            await instructors.AddAsync(instructor, cancellationToken);
        }
        catch (DataIntegrityException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var created = await instructors.GetByIdAsync(instructor.Id, cancellationToken);
        return CreatedAtAction(
            nameof(GetById), new { id = instructor.Id }, ToModel(created!));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InstructorModel>> Update(
        Guid id,
        InstructorWriteModel model,
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

        var employeeNumber = model.EmployeeNumber.Trim().ToUpperInvariant();
        if (await instructors.EmployeeNumberExistsAsync(
                employeeNumber, id, cancellationToken))
        {
            return Conflict(new { message = "Ya existe un instructor con ese código." });
        }

        Apply(instructor, model, employeeNumber);
        try
        {
            await instructors.UpdateAsync(instructor, cancellationToken);
        }
        catch (DataIntegrityException exception)
        {
            return Conflict(new { message = exception.Message });
        }

        var updated = await instructors.GetByIdAsync(id, cancellationToken);
        return Ok(ToModel(updated!));
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
        catch (DataIntegrityException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    private static void Apply(
        Instructor instructor,
        InstructorWriteModel model,
        string employeeNumber)
    {
        instructor.EmployeeNumber = employeeNumber;
        instructor.FirstName = model.FirstName.Trim();
        instructor.LastName = model.LastName.Trim();
        instructor.Email = model.Email.Trim().ToLowerInvariant();
        instructor.PhoneNumber = model.PhoneNumber?.Trim();
        instructor.DepartmentId = model.DepartmentId;
    }

    private static InstructorModel ToModel(Instructor instructor) =>
        new(
            instructor.Id,
            instructor.EmployeeNumber,
            instructor.FirstName,
            instructor.LastName,
            instructor.FullName,
            instructor.Email,
            instructor.PhoneNumber,
            instructor.DepartmentId,
            instructor.Department.Name,
            instructor.HireDateUtc,
            instructor.CreatedAtUtc,
            instructor.UpdatedAtUtc);
}
