using System.ComponentModel.DataAnnotations;

namespace BiciTaller.Infrastructure.Models;

public sealed record CategoriaModel(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed class CategoriaWriteModel
{
    [Required, StringLength(20, MinimumLength = 2)]
    public string Codigo { get; init; } = string.Empty;

    [Required, StringLength(120, MinimumLength = 2)]
    public string Nombre { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; init; }
}
