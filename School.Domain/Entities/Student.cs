using School.Domain.Core;

namespace School.Domain.Entities;

public sealed class Student : Person
{
    public string StudentNumber { get; set; } = string.Empty;
    public DateTime EnrollmentDateUtc { get; set; } = DateTime.UtcNow;
    public Guid DepartmentId { get; set; }

    public Department Department { get; set; } = null!;
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
