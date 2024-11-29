using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Infrastructure.Persistance;
using TaskManagementSystem.Domain.Interfaces.Repositories;
using TaskManagementSystem.Infrastructure.Repositories;
using RabbitMQ.Client;
using TaskManagementSystem.Infrastructure.ServiceBus;
using TaskManagementSystem.Domain.Interfaces.ServiceBus;

namespace TaskManagementSystem.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("TaskManagementSystem.Infrastructure")));

        services.AddSingleton<IConnection>(provider =>
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            return factory.CreateConnection();
        });

        services.AddSingleton<IServiceBusHandler, ServiceBusHandler>();
        services.AddHostedService<MessageProcessingService>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        return services;
    }
}