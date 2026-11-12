using System.ComponentModel.DataAnnotations;

namespace School.Infrastructure.Models;

public sealed record DepartmentModel(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed class DepartmentWriteModel
{
    [Required, StringLength(20, MinimumLength = 2)]
    public string Code { get; init; } = string.Empty;

    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }
}
