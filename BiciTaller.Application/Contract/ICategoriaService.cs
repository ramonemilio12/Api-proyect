using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Categoria;

namespace BiciTaller.Application.Contract;

public interface ICategoriaService
    : IBaseService<CategoriaDto, CategoriaCreateDto, CategoriaUpdateDto>
{
}
