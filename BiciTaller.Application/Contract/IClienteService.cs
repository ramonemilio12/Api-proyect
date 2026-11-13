using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Cliente;

namespace BiciTaller.Application.Contract;

public interface IClienteService
    : IBaseService<ClienteDto, ClienteCreateDto, ClienteUpdateDto>
{
}
