using School.Application.Dtos;

namespace School.Application.Dtos.Course;

public sealed class CourseDto : DtoBase
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public Guid? InstructorId { get; set; }
    public string? InstructorName { get; set; }
}
