using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Tecnico;

namespace BiciTaller.Application.Contract;

public interface ITecnicoService
    : IBaseService<TecnicoDto, TecnicoCreateDto, TecnicoUpdateDto>
{
}
