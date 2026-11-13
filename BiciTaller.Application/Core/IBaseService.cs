namespace BiciTaller.Application.Core;

public interface IBaseService<TDto, TCreateDto, TUpdateDto>
{
    Task<ServiceResult<IReadOnlyList<TDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ServiceResult<TDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<TDto>> CreateAsync(
        TCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<TDto>> UpdateAsync(
        Guid id,
        TUpdateDto dto,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
