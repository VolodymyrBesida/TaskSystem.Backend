using AutoMapper;
using Moq;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Interfaces.Repositories;
using TaskManagementSystem.Domain.Interfaces.ServiceBus;

using EntityTask = TaskManagementSystem.Domain.Entities.Task;
using EntityTaskStatus = TaskManagementSystem.Domain.Enums.TaskStatus;

namespace TaskManagementSystem.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly Mock<IServiceBusHandler> _mockServiceBusHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _mockServiceBusHandler = new Mock<IServiceBusHandler>();
        _mockMapper = new Mock<IMapper>();

        _taskService = new TaskService(_mockTaskRepository.Object, _mockServiceBusHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddTaskAsync_ShouldAddTask()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Name = "Test Task",
            Description = "Test Description",
            Status = "InProgress"
        };

        var taskEntity = new EntityTask
        {
            Id = 1,
            Name = createTaskDto.Name,
            Description = createTaskDto.Description,
            Status = EntityTaskStatus.InProgress
        };

        var taskDto = new TaskDto
        {
            Id = 1,
            Name = createTaskDto.Name,
            Description = createTaskDto.Description,
            Status = createTaskDto.Status
        };

        _mockMapper.Setup(m => m.Map<EntityTask>(It.IsAny<CreateTaskDto>())).Returns(taskEntity);
        _mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<EntityTask>())).Returns(taskDto);
        _mockTaskRepository.Setup(repo => repo.AddAsync(It.IsAny<EntityTask>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.AddTaskAsync(createTaskDto, CancellationToken.None);

        // Assert
        _mockTaskRepository.Verify(repo => repo.AddAsync(It.IsAny<EntityTask>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceBusHandler.Verify(bus => bus.SendMessage(It.IsAny<object>()), Times.Once);
        Assert.Equal(taskDto.Id, result.Id);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_ShouldUpdateTaskStatus()
    {
        // Arrange
        var statusDto = new TaskStatusDto { Status = "Completed" };
        var taskEntity = new EntityTask
        {
            Id = 1,
            Name = "Test Task",
            Description = "Test Description",
            Status = EntityTaskStatus.InProgress
        };

        var updatedTaskEntity = new EntityTask
        {
            Id = 1,
            Name = "Test Task",
            Description = "Test Description",
            Status = EntityTaskStatus.Completed
        };

        var taskDto = new TaskDto
        {
            Id = 1,
            Name = "Test Task",
            Description = "Test Description",
            Status = "Completed"
        };

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(taskEntity);
        _mockMapper.Setup(m => m.Map<TaskDto>(It.IsAny<EntityTask>())).Returns(taskDto);
        _mockTaskRepository.Setup(repo => repo.UpdateAsync(It.IsAny<EntityTask>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.UpdateTaskStatusAsync(1, statusDto, CancellationToken.None);

        // Assert
        _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<EntityTask>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceBusHandler.Verify(bus => bus.SendMessage(It.IsAny<object>()), Times.Once);
        Assert.Equal("Completed", result.Status);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnAllTasks()
    {
        // Arrange
        var taskEntities = new List<EntityTask>
        {
            new EntityTask
            {
                Id = 1,
                Name = "Test Task 1",
                Description = "Test Description 1",
                Status = EntityTaskStatus.Completed
            },
            new EntityTask
            {
                Id = 2,
                Name = "Test Task 2",
                Description = "Test Description 2",
                Status = EntityTaskStatus.InProgress
            }
        };

        var taskDtos = new List<TaskDto>
        {
            new TaskDto
            {
                Id = 1,
                Name = "Test Task 1",
                Description = "Test Description 1",
                Status = "Completed"
            },
            new TaskDto
            {
                Id = 2,
                Name = "Test Task 2",
                Description = "Test Description 2",
                Status = "InProgress"
            }
        };

        _mockTaskRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(taskEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<EntityTask>>())).Returns(taskDtos);

        // Act
        var result = await _taskService.GetAllTasksAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("Test Task 1", result.First().Name);
    }
}