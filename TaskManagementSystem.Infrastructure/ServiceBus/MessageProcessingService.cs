using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using TaskManagementSystem.Domain.Interfaces.ServiceBus;

namespace TaskManagementSystem.Infrastructure.ServiceBus;

public class MessageProcessingService : BackgroundService
{
    private readonly IServiceBusHandler _serviceBusHandler;

    public MessageProcessingService(IServiceBusHandler serviceBusHandler)
    {
        _serviceBusHandler = serviceBusHandler;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serviceBusHandler.ReceiveDeadLetterQueueMessages();
        _serviceBusHandler.ReceiveMessage<dynamic>(message =>
        {
            dynamic taskMessage = JsonConvert.DeserializeObject(message.ToString());
            Console.WriteLine($"Received message: {message}");

            if (taskMessage != null)
            {
                Console.WriteLine($"Received message: {taskMessage}");

                if (taskMessage.Action == "TaskAdded")
                    Console.WriteLine($"Task {taskMessage.TaskId} added with name {taskMessage.Name}");
                else if (taskMessage.Action == "TaskUpdated")
                    Console.WriteLine($"Task {taskMessage.TaskId} updated to status {taskMessage.NewStatus}");
            }
        });
        //add manual acknowledge to make it more controlled in further operations
        return Task.CompletedTask;
    }
}