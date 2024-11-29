namespace TaskManagementSystem.Domain.Entities;

public class Task
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Enums.TaskStatus Status { get; set; }
    public string? AssignedTo { get; set; }
}