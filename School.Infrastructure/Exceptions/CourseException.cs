namespace School.Infrastructure.Exceptions;

public sealed class CourseException(string message) : Exception(message)
{
}
