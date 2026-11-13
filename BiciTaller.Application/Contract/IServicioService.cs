using BiciTaller.Application.Core;
using BiciTaller.Application.Dtos.Servicio;

namespace BiciTaller.Application.Contract;

public interface IServicioService
    : IBaseService<ServicioDto, ServicioCreateDto, ServicioUpdateDto>
{
}
