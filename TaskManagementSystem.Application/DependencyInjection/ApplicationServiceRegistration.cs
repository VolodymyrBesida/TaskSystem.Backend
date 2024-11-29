using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Application.Interfaces.Services;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        return services;
    }
}