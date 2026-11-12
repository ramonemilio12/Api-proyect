namespace School.Infrastructure.Exceptions;

public sealed class DuplicateEntityException(string message) : Exception(message)
{
}
