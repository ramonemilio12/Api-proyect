namespace BiciTaller.Application.Core;

public sealed class ServiceResult<T>
{
    public ServiceResult(bool success, T data, IReadOnlyList<string> errors)
    {
        Success = success;
        Data = data;
        Errors = errors;
    }

    public bool Success { get; }
    public T Data { get; }
    public IReadOnlyList<string> Errors { get; }

    public static ServiceResult<T> Ok(T data) =>
        new(true, data, Array.Empty<string>());

    public static ServiceResult<T> Fail(params string[] errors) =>
        new(false, default!, errors);
}
