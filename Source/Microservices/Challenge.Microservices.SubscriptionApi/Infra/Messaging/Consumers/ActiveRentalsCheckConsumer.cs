using Challenge.Microservices.SubscriptionApi.Infra.Data.Repositories;
using Challenge.Microservices.SubscriptionApi.Infra.Messaging.Requests;
using Challenge.Microservices.SubscriptionApi.Infra.Messaging.Responses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Challenge.Microservices.SubscriptionApi.Infra.Messaging.Consumers
{
    /// <summary>
    /// Background service that consumes requests to check if a motorbike has active rentals
    /// </summary>
    public class ActiveRentalsCheckConsumer(
        IConnection connection,
        IServiceProvider serviceProvider,
        ILogger<ActiveRentalsCheckConsumer> logger) : BackgroundService
    {
        private const string QueueName = "subscription.check-active-rentals";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            using var channel = connection.CreateModel();

            // Declare queue for active rentals checks
            channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            // Set prefetch to process one message at a time
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = ea.BasicProperties.CorrelationId;
                replyProps.ContentType = "application/json";

                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var request = JsonSerializer.Deserialize<CheckActiveRentalsRequest>(message);

                    logger.LogInformation(
                        "Received request to check active rentals. MotorbikeIdentifier: {Identifier}, CorrelationId: {CorrelationId}",
                        request?.MotorbikeIdentifier, ea.BasicProperties.CorrelationId);

                    // Check if motorbike has active subscriptions
                    using var scope = serviceProvider.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

                    var hasActiveRentals = await repository.HasActiveSubscriptionAsync(request?.MotorbikeIdentifier ?? "");

                    // Prepare response
                    var response = new CheckActiveRentalsResponse
                    {
                        HasActiveRentals = hasActiveRentals,
                        MotorbikeIdentifier = request?.MotorbikeIdentifier
                    };

                    var responseBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

                    // Send response back
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: ea.BasicProperties.ReplyTo,
                        basicProperties: replyProps,
                        body: responseBody);

                    logger.LogInformation(
                        "Sent response for active rentals check. MotorbikeIdentifier: {Identifier}, HasActiveRentals: {HasActiveRentals}",
                        request?.MotorbikeIdentifier, hasActiveRentals);

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing active rentals check request");

                    // Send error response
                    var errorResponse = new CheckActiveRentalsResponse
                    {
                        HasActiveRentals = false,
                        MotorbikeIdentifier = null
                    };

                    var responseBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(errorResponse));

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: ea.BasicProperties.ReplyTo,
                        basicProperties: replyProps,
                        body: responseBody);

                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            logger.LogInformation("Active rentals check consumer started");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}