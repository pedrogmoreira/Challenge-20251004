using AutoMapper;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;
using Challenge.Microservices.MotorbikeApi.Infra.Messaging.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Challenge.Microservices.MotorbikeApi.Infra.Messaging.Publishers
{

    /// <summary>
    /// RabbitMQ message publisher for motorbike events
    /// </summary>
    public class RabbitMqMotorbikePublisher(
        IMapper mapper,
        IConnection connection,
        ILogger<RabbitMqMotorbikePublisher> logger) : IMotorbikeMessagePublisher
    {
        private const string ExchangeName = "motorbike.events";
        private const string RoutingKey = "motorbike.registered";

        public async Task PublishMotorbikeRegisteredAsync(Motorbike motorbike)
        {
            try
            {
                using var channel = connection.CreateModel();

                // Declare exchange
                channel.ExchangeDeclare(
                    exchange: ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                var message = mapper.Map<MotorbikeRegisteredEvent>(motorbike);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: RoutingKey,
                    basicProperties: properties,
                    body: body);

                logger.LogInformation(
                    "Published motorbike registered event. MotorbikeId: {MotorbikeId}, Year: {Year}",
                    motorbike.Id, motorbike.Year);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error publishing motorbike registered event");
                throw;
            }
        }
    }
}