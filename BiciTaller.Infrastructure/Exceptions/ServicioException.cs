namespace BiciTaller.Infrastructure.Exceptions;

public sealed class ServicioException(string message) : Exception(message)
{
}
