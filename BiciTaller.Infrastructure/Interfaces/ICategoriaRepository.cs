using BiciTaller.Domain.Entities;
using BiciTaller.Domain.Repository;

namespace BiciTaller.Infrastructure.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<bool> ExisteCodigoAsync(
        string codigo,
        Guid? exceptId = null,
        CancellationToken cancellationToken = default);
}
