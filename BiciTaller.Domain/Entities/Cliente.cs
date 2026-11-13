using BiciTaller.Domain.Core;

namespace BiciTaller.Domain.Entities;

public sealed class Cliente : Persona
{
    public string NumeroCliente { get; set; } = string.Empty;
    public DateTime FechaRegistroUtc { get; set; } = DateTime.UtcNow;
    public Guid CategoriaId { get; set; }

    public Categoria Categoria { get; set; } = null!;
    public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
}
