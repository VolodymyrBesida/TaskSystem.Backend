namespace TaskManagementSystem.Infrastructure.Settings;

public class ServiceBusSettings
{
    public int MaxRetryCount { get; set; }
    public string TasksQueueName { get; set; }
    public string DeadLetterQueueName { get; set; }
}