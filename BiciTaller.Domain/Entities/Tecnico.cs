using BiciTaller.Domain.Core;

namespace BiciTaller.Domain.Entities;

public sealed class Tecnico : Persona
{
    public string NumeroEmpleado { get; set; } = string.Empty;
    public DateTime FechaIngresoUtc { get; set; } = DateTime.UtcNow;
    public Guid CategoriaId { get; set; }

    public Categoria Categoria { get; set; } = null!;
    public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
}
