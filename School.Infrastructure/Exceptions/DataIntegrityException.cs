namespace School.Infrastructure.Exceptions;

public sealed class DataIntegrityException(string message, Exception innerException)
    : Exception(message, innerException)
{
}
