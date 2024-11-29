namespace TaskManagementSystem.Domain.Interfaces.ServiceBus;

public interface IServiceBusHandler
{
    void SendMessage<T>(T message);
    void ReceiveMessage<T>(Action<T> onMessage);
    void ReceiveDeadLetterQueueMessages();
}