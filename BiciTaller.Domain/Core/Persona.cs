namespace BiciTaller.Domain.Core;

public abstract class Persona : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }

    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
}
