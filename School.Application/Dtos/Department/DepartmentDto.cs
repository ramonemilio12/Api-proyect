using School.Application.Dtos;

namespace School.Application.Dtos.Department;

public sealed class DepartmentDto : DtoBase
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
