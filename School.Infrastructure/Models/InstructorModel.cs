using System.ComponentModel.DataAnnotations;

namespace School.Infrastructure.Models;

public sealed record InstructorModel(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? PhoneNumber,
    Guid DepartmentId,
    string DepartmentName,
    DateTime HireDateUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed class InstructorWriteModel
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
