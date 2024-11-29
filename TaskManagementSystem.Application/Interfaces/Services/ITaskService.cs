using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Interfaces.Services;

public interface ITaskService
{
    Task<TaskDto> AddTaskAsync(CreateTaskDto createTaskDto, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateTaskStatusAsync(int id, TaskStatusDto statusDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default);
}