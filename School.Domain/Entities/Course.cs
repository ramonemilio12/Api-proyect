using School.Domain.Core;

namespace School.Domain.Entities;

public sealed class Course : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid? InstructorId { get; set; }

    public Department Department { get; set; } = null!;
    public Instructor? Instructor { get; set; }
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
