using System.ComponentModel.DataAnnotations;

namespace BiciTaller.Application.Dtos.Cliente;

public sealed class ClienteUpdateDto
{
    [Required(ErrorMessage = "El número de cliente es obligatorio.")]
    [StringLength(30, MinimumLength = 2,
        ErrorMessage = "El número de cliente debe tener entre 2 y 30 caracteres.")]
    public string NumeroCliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, MinimumLength = 1,
        ErrorMessage = "El nombre debe tener entre 1 y 80 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(80, MinimumLength = 1,
        ErrorMessage = "El apellido debe tener entre 1 y 80 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(200, ErrorMessage = "El correo no puede exceder 200 caracteres.")]
    public string Correo { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(30, ErrorMessage = "El teléfono no puede exceder 30 caracteres.")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public Guid CategoriaId { get; set; }
}
