using Microsoft.Extensions.DependencyInjection;
using BiciTaller.Application.Contract;
using BiciTaller.Application.Service;

namespace BiciTaller.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IServicioService, ServicioService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<ITecnicoService, TecnicoService>();

        return services;
    }
}
