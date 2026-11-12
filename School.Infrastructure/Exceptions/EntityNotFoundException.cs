namespace School.Infrastructure.Exceptions;

public sealed class EntityNotFoundException(string entityName, Guid id)
    : Exception($"{entityName} con ID '{id}' no fue encontrado.")
{
}
