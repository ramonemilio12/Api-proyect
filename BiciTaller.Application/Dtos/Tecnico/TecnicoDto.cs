using BiciTaller.Application.Dtos;

namespace BiciTaller.Application.Dtos.Tecnico;

public sealed class TecnicoDto : DtoBase
{
    public string NumeroEmpleado { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public Guid CategoriaId { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public DateTime FechaIngresoUtc { get; set; }
}
