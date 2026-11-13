using System.ComponentModel.DataAnnotations;

namespace School.Application.Core;

public abstract class BaseService
{
    protected ServiceResult<T> Success<T>(T data) =>
        new(true, data, Array.Empty<string>());

    protected ServiceResult<T> Failure<T>(params string[] errors) =>
        new(false, default!, errors);

    protected bool TryValidate(object instance, out IReadOnlyList<string> errors)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(instance);
        var isValid = Validator.TryValidateObject(
            instance, context, validationResults, validateAllProperties: true);

        errors = validationResults
            .Where(result => result.ErrorMessage is not null)
            .Select(result => result.ErrorMessage!)
            .ToList();

        return isValid;
    }
}
