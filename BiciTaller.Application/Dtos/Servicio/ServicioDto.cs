using BiciTaller.Application.Dtos;

namespace BiciTaller.Application.Dtos.Servicio;

public sealed class ServicioDto : DtoBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public Guid CategoriaId { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public Guid? TecnicoId { get; set; }
    public string? NombreTecnico { get; set; }
}
