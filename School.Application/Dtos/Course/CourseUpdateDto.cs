using System.ComponentModel.DataAnnotations;

namespace School.Application.Dtos.Course;

public sealed class CourseUpdateDto
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(20, MinimumLength = 2,
        ErrorMessage = "El código debe tener entre 2 y 20 caracteres.")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 30, ErrorMessage = "Los créditos deben estar entre 1 y 30.")]
    public int Credits { get; set; }

    [Required(ErrorMessage = "El departamento es obligatorio.")]
    public Guid DepartmentId { get; set; }

    public Guid? InstructorId { get; set; }
}
