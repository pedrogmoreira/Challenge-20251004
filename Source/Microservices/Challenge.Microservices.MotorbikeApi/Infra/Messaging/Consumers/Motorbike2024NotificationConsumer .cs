using AutoMapper;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories;
using Challenge.Microservices.MotorbikeApi.Infra.Messaging.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Challenge.Microservices.MotorbikeApi.Infra.Messaging.Consumers
{
    /// <summary>
    /// Background service to consume motorbike events and store 2024 year notifications
    /// Requirement: When a 2024 motorbike is registered, store a notification
    /// </summary>
    public class Motorbike2024NotificationConsumer(
        IMapper mapper,
        IConnection connection,
        IServiceProvider serviceProvider,
        ILogger<Motorbike2024NotificationConsumer> logger) : BackgroundService
    {
        private const string QueueName = "motorbike.2024.notifications";
        private const string ExchangeName = "motorbike.events";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            using var channel = connection.CreateModel();

            // Declare exchange (idempotent)
            channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // Declare queue for 2024 notifications
            channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            // Bind queue to exchange
            channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: "motorbike.registered");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var motorbikeEvent = JsonSerializer.Deserialize<MotorbikeRegisteredEvent>(message);

                    // Filter: Only process 2024 motorbikes (requirement from challenge)
                    if (motorbikeEvent?.Year == 2024)
                    {
                        logger.LogInformation(
                            "Received 2024 motorbike registration. MotorbikeId: {MotorbikeId}, Model: {Model}",
                            motorbikeEvent.MotorbkeIdentifier, motorbikeEvent.Model);

                        // Store notification in MongoDB
                        using var scope = serviceProvider.CreateScope();
                        var notificationRepository = scope.ServiceProvider
                            .GetRequiredService<IMotorbikeNotificationRepository>();

                        var notification = mapper.Map<MotorbikeNotification>(motorbikeEvent);

                        await notificationRepository.AddAsync(notification);

                        logger.LogInformation(
                            "2024 motorbike notification stored. MotorbikeId: {MotorbikeId}",
                            motorbikeEvent.MotorbkeIdentifier);
                    }

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing motorbike event");
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            logger.LogInformation("Motorbike 2024 notification consumer started");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}