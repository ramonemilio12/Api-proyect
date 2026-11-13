using BiciTaller.Application.Dtos;

namespace BiciTaller.Application.Dtos.Cliente;

public sealed class ClienteDto : DtoBase
{
    public string NumeroCliente { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public Guid CategoriaId { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public DateTime FechaRegistroUtc { get; set; }
}
