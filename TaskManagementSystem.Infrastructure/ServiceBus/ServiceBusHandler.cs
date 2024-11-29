using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TaskManagementSystem.Domain.Interfaces.ServiceBus;
using TaskManagementSystem.Infrastructure.Settings;

namespace TaskManagementSystem.Infrastructure.ServiceBus;

public class ServiceBusHandler : IServiceBusHandler
{
    private readonly IModel _channel;
    private readonly ServiceBusSettings _serviceBusSettings;

    public ServiceBusHandler(IConnection connection, IOptions<ServiceBusSettings> serviceBusSettings)
    {
        _channel = connection.CreateModel();
        _serviceBusSettings = serviceBusSettings.Value;

        _channel.QueueDeclare(queue: _serviceBusSettings.TasksQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: _serviceBusSettings.DeadLetterQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void SendMessage<T>(T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish(exchange: "", routingKey: _serviceBusSettings.TasksQueueName, basicProperties: null, body: body);
    }

    public void ReceiveMessage<T>(Action<T> onMessage)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, args) =>
        {
            var body = args.Body.ToArray();
            var retryCount = GetRetryCount(args.BasicProperties.Headers);

            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));

            try
            {
                //throw new Exception("Simulated failure"); -- for testing dlq
                if (message is not null)
                    onMessage(message);
                _channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
            }
            catch (Exception)
            {
                HandleProcessingFailure(args, body, retryCount);
            }
        };

        // Use manual acknowledgment if going to implement custom logic
        _channel.BasicConsume(queue: _serviceBusSettings.TasksQueueName, autoAck: false, consumer: consumer);
    }

    public void ReceiveDeadLetterQueueMessages()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"DLQ Message: {message}");

            // Acknowledge that the message was received and processed
            _channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: _serviceBusSettings.DeadLetterQueueName, autoAck: false, consumer: consumer);
    }

    private int GetRetryCount(IDictionary<string, object>? headers)
    {
        if (headers is not null && headers.TryGetValue("x-retry-count", out var retryHeader))
            return int.TryParse(Encoding.UTF8.GetString((byte[])retryHeader), out var retryCount) ? retryCount : 0;
        return 0;
    }

    private void HandleProcessingFailure(BasicDeliverEventArgs args, byte[] body, int retryCount)
    {
        retryCount++;
        if (retryCount <= _serviceBusSettings.MaxRetryCount)
            PublishToQueue(_serviceBusSettings.TasksQueueName, body, retryCount);
        else
            PublishToQueue(_serviceBusSettings.DeadLetterQueueName, body, retryCount, isDlq: true);

        // Reject the original message without requeueing
        _channel.BasicNack(deliveryTag: args.DeliveryTag, multiple: false, requeue: false);
    }

    private void PublishToQueue(string queue, byte[] body, int retryCount, bool isDlq = false)
    {
        var properties = _channel.CreateBasicProperties();
        properties.Headers = new Dictionary<string, object>
        {
            { isDlq ? "x-original-retry-count" : "x-retry-count", Encoding.UTF8.GetBytes(retryCount.ToString()) }
        };

        _channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: properties, body: body);
    }
}