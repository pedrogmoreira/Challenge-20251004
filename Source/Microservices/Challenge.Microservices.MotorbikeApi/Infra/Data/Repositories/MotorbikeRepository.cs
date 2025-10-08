using Challenge.Common.Data.Mongo.Repositories;
using Challenge.Microservices.MotorbikeApi.Infra.Data.Entities;
using Challenge.Microservices.MotorbikeApi.Infra.Messaging.Requests;
using Challenge.Microservices.MotorbikeApi.Infra.Messaging.Responses;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Challenge.Microservices.MotorbikeApi.Infra.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Motorbike entity
    /// </summary>
    public class MotorbikeRepository(
        IMongoDatabase database, 
        IConnection rabbitConnection, 
        ILogger<MotorbikeRepository> logger) 
        : MongoRepository<Motorbike>(database, "Motorbikes"), IMotorbikeRepository
    {
        public async Task<bool?> DisableByIdentifierAsync(string identifier)
        {             
            var motorbike = await GetByIdentifierAsync(identifier);

            if (motorbike is null)
            {
                return false;
            }

            motorbike.Active = false;

            return await UpdateAsync(motorbike);
        }

        public IEnumerable<Motorbike>? GetActive()
        {
            return GetAll().Where(x => x.Active is true);
        }

        public async Task<Motorbike?> GetByIdentifierAsync(string identifier)
        {
            var filter = Builders<Motorbike>.Filter.Eq(m => m.Identifier, identifier);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Motorbike?> GetByLicensePlateAsync(string licensePlate)
        {
            var filter = Builders<Motorbike>.Filter.Eq(m => m.LicensePlate, licensePlate);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> LicensePlateExistsAsync(string licensePlate)
        {
            var filter = Builders<Motorbike>.Filter.Eq(m => m.LicensePlate, licensePlate);
            return await _collection.Find(filter).AnyAsync();
        }

        public async Task<bool> IdentifierExistsAsync(string identifier)
        {
            var filter = Builders<Motorbike>.Filter.Eq(m => m.Identifier, identifier);
            return await _collection.Find(filter).AnyAsync();
        }

        /// <summary>
        /// Checks if a motorbike has active rentals by sending a RPC request via RabbitMQ
        /// </summary>
        public async Task<bool> HasActiveRentalsAsync(string identifier)
        {
            try
            {
                using var channel = rabbitConnection.CreateModel();

                // Declare temporary response queue
                var replyQueueName = channel.QueueDeclare(
                    queue: "",
                    durable: false,
                    exclusive: true,
                    autoDelete: true).QueueName;

                var correlationId = Guid.NewGuid().ToString();
                var tcs = new TaskCompletionSource<bool>();

                // Consumer for response
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        var responseBody = ea.Body.ToArray();
                        var responseMessage = Encoding.UTF8.GetString(responseBody);
                        var response = JsonSerializer.Deserialize<CheckActiveRentalsResponse>(responseMessage);

                        tcs.TrySetResult(response?.HasActiveRentals ?? false);
                    }
                };

                channel.BasicConsume(
                    queue: replyQueueName,
                    autoAck: true,
                    consumer: consumer);

                // Send request
                var requestQueue = "subscription.check-active-rentals";

                // Declare request queue (idempotent)
                channel.QueueDeclare(
                    queue: requestQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                var request = new CheckActiveRentalsRequest
                {
                    MotorbikeIdentifier = identifier
                };

                var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));

                var properties = channel.CreateBasicProperties();
                properties.CorrelationId = correlationId;
                properties.ReplyTo = replyQueueName;
                properties.ContentType = "application/json";
                properties.Expiration = "5000"; // 5 seconds timeout

                channel.BasicPublish(
                    exchange: "",
                    routingKey: requestQueue,
                    basicProperties: properties,
                    body: messageBody);

                logger.LogInformation(
                    "Sent RabbitMQ request to check active rentals. Identifier: {Identifier}, CorrelationId: {CorrelationId}",
                    identifier, correlationId);

                // Wait for response with timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                cts.Token.Register(() => tcs.TrySetResult(false));

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error checking active rentals via RabbitMQ. Identifier: {Identifier}",
                    identifier);

                // On error, assume no active rentals (fail-open approach)
                // In production, you might want to fail-closed (return true) depending on requirements
                return false;
            }
        }
    }
}