using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BiciTaller.Domain.Repository;
using BiciTaller.Infrastructure.Context;
using BiciTaller.Infrastructure.Core;
using BiciTaller.Infrastructure.Interfaces;
using BiciTaller.Infrastructure.Repositories;

namespace BiciTaller.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<BiciTallerContext>(
            options => options.UseSqlite(connectionString));
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IServicioRepository, ServicioRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
