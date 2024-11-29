using TaskManagementSystem.API;
using TaskManagementSystem.API.Endpoints;
using TaskManagementSystem.Application;
using TaskManagementSystem.Application.DependencyInjection;
using TaskManagementSystem.Infrastructure.DependencyInjection;
using TaskManagementSystem.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBusSettings"));

builder.Services.AddPresentation();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin()  // Allows all origins
              .AllowAnyHeader()  // Allows any headers
              .AllowAnyMethod(); // Allows any HTTP method (GET, POST, etc.)
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseCors("AllowAnyOrigin");
app.UseDefaultFiles();
// Map endpoints
app.MapTasksEndpoints();

app.Run();