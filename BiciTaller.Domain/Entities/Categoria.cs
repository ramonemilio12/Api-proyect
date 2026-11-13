using BiciTaller.Domain.Core;

namespace BiciTaller.Domain.Entities;

public sealed class Categoria : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public ICollection<Tecnico> Tecnicos { get; set; } = new List<Tecnico>();
}
