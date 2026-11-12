using School.Domain.Core;

namespace School.Domain.Entities;

public sealed class Department : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
}
