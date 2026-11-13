using BiciTaller.Domain.Core;

namespace BiciTaller.Domain.Entities;

public sealed class Servicio : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid? TecnicoId { get; set; }

    public Categoria Categoria { get; set; } = null!;
    public Tecnico? Tecnico { get; set; }
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
