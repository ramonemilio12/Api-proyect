using System.ComponentModel.DataAnnotations;

namespace School.Application.Dtos.Student;

public sealed class StudentCreateDto
{
    [Required(ErrorMessage = "El número de estudiante es obligatorio.")]
    [StringLength(30, MinimumLength = 2,
        ErrorMessage = "El número de estudiante debe tener entre 2 y 30 caracteres.")]
    public string StudentNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, MinimumLength = 1,
        ErrorMessage = "El nombre debe tener entre 1 y 80 caracteres.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(80, MinimumLength = 1,
        ErrorMessage = "El apellido debe tener entre 1 y 80 caracteres.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(200, ErrorMessage = "El correo no puede exceder 200 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(30, ErrorMessage = "El teléfono no puede exceder 30 caracteres.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "El departamento es obligatorio.")]
    public Guid DepartmentId { get; set; }
}
