using AutoMapper;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces.Services;
using TaskManagementSystem.Domain.Interfaces.Repositories;
using TaskManagementSystem.Domain.Interfaces.ServiceBus;

using EntityTask = TaskManagementSystem.Domain.Entities.Task;
using EntityTaskStatus = TaskManagementSystem.Domain.Enums.TaskStatus;

namespace TaskManagementSystem.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IServiceBusHandler _serviceBusHandler;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository,
        IServiceBusHandler serviceBusHandler,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _serviceBusHandler = serviceBusHandler;
        _mapper = mapper;
    }

    public async Task<TaskDto> AddTaskAsync(CreateTaskDto createTaskDto, CancellationToken cancellationToken = default)
    {
        var task = _mapper.Map<EntityTask>(createTaskDto);
        task.Status = Enum.Parse<EntityTaskStatus>(createTaskDto.Status);
        await _taskRepository.AddAsync(task, cancellationToken);

        _serviceBusHandler.SendMessage(new
        {
            Action = "TaskAdded",
            TaskId = task.Id,
            Name = task.Name,
            Description = task.Description
        });

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> UpdateTaskStatusAsync(int id, TaskStatusDto statusDto, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null)
            throw new KeyNotFoundException($"Task with ID {id} not found.");

        task.Status = Enum.Parse<EntityTaskStatus>(statusDto.Status);
        await _taskRepository.UpdateAsync(task, cancellationToken);

        _serviceBusHandler.SendMessage(new
        {
            Action = "TaskUpdated",
            TaskId = task.Id,
            NewStatus = task.Status.ToString()
        });

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
    }
}