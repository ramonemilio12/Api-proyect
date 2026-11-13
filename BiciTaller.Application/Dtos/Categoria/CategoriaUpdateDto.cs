using System.ComponentModel.DataAnnotations;

namespace BiciTaller.Application.Dtos.Categoria;

public sealed class CategoriaUpdateDto
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(20, MinimumLength = 2,
        ErrorMessage = "El código debe tener entre 2 y 20 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120, MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 120 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500,
        ErrorMessage = "La descripción no puede exceder 500 caracteres.")]
    public string? Descripcion { get; set; }
}
