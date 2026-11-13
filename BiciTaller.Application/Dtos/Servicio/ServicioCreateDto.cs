using System.ComponentModel.DataAnnotations;

namespace BiciTaller.Application.Dtos.Servicio;

public sealed class ServicioCreateDto
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(20, MinimumLength = 2,
        ErrorMessage = "El código debe tener entre 2 y 20 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, 100000, ErrorMessage = "El precio debe estar entre 1 y 100000.")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public Guid CategoriaId { get; set; }

    public Guid? TecnicoId { get; set; }
}
