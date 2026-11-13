using System.ComponentModel.DataAnnotations;

namespace BiciTaller.Infrastructure.Models;

public sealed record ServicioModel(
    Guid Id,
    string Codigo,
    string Nombre,
    decimal Precio,
    Guid CategoriaId,
    string NombreCategoria,
    Guid? TecnicoId,
    string? NombreTecnico,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed class ServicioWriteModel
{
    [Required, StringLength(20, MinimumLength = 2)]
    public string Codigo { get; init; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 2)]
    public string Nombre { get; init; } = string.Empty;

    [Range(1, 100000, ErrorMessage = "El precio debe estar entre 1 y 100000.")]
    public decimal Precio { get; init; }

    public Guid CategoriaId { get; init; }
    public Guid? TecnicoId { get; init; }
}
