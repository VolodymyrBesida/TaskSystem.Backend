namespace TaskManagementSystem.Application.DTOs;

public class CreateTaskDto : TaskDefaultDto;

public class TaskStatusDto
{
    public string Status { get; set; }
}

public class TaskDto : TaskDefaultDto
{
    public int Id { get; set; }
}

public class TaskDefaultDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string? AssignedTo { get; set; }
}