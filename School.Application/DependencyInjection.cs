using Microsoft.Extensions.DependencyInjection;
using School.Application.Contract;
using School.Application.Service;

namespace School.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDepartamentService, DepartamentService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IInstructorService, InstructorService>();

        return services;
    }
}
