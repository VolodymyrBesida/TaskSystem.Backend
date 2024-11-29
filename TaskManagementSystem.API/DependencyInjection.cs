namespace TaskManagementSystem.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Task Management API",
                Version = "v1",
                Description = "API for managing tasks"
            });
        });
        services.AddControllers();

        return services;
    }
}