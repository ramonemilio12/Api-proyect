using BiciTaller.Application.Dtos;

namespace BiciTaller.Application.Dtos.Categoria;

public sealed class CategoriaDto : DtoBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
