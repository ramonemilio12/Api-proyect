using System.ComponentModel.DataAnnotations;

namespace School.Infrastructure.Models;

public sealed record CursoModel(
    Guid Id,
    string Code,
    string Name,
    int Credits,
    Guid DepartmentId,
    string DepartmentName,
    Guid? InstructorId,
    string? InstructorName,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed class CourseWriteModel
{
    [Required, StringLength(20, MinimumLength = 2)]
    public string Code { get; init; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Range(1, 30)]
    public int Credits { get; init; }

    public Guid DepartmentId { get; init; }
    public Guid? InstructorId { get; init; }
}
