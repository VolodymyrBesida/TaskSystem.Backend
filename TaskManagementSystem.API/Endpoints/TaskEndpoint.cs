using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces.Services;

namespace TaskManagementSystem.API.Endpoints;

public static class TaskEndpoint
{
    public static IEndpointRouteBuilder MapTasksEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks");

        group.MapGet("/", GetTasksAsync);
        group.MapPost("/", CreateTaskAsync);
        group.MapPut("/{id:int}", UpdateTaskAsync);

        return app;
    }

    private static async Task<IResult> GetTasksAsync(ITaskService _service, CancellationToken cancellationToken = default)
    {
        var tasks = await _service.GetAllTasksAsync(cancellationToken);
        return Results.Ok(tasks);
    }

    private static async Task<IResult> CreateTaskAsync(ITaskService _service, CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var task = await _service.AddTaskAsync(dto, cancellationToken);
        return Results.Created($"/tasks/{task.Id}", task);
    }

    private static async Task<IResult> UpdateTaskAsync(ITaskService _service, int id, TaskStatusDto dto, CancellationToken cancellationToken = default)
    {
        await _service.UpdateTaskStatusAsync(id, dto, cancellationToken);
        return Results.NoContent();
    }
}