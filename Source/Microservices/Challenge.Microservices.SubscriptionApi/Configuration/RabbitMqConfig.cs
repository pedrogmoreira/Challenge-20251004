using Challenge.Microservices.SubscriptionApi.Infra.Messaging.Consumers;
using RabbitMQ.Client;

namespace Challenge.Microservices.SubscriptionApi.Configuration
{
    /// <summary>
    /// Configuration for RabbitMQ connection
    /// </summary>
    public static class RabbitMqConfig
    {
        /// <summary>
        /// Configures RabbitMQ connection and consumers
        /// </summary>
        public static IServiceCollection SetupRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = configuration.GetValue<int>("RabbitMQ:Port", 5672),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            var connection = factory.CreateConnection();
            services.AddSingleton(connection);

            // Register consumer for active rentals checks
            services.AddHostedService<ActiveRentalsCheckConsumer>();

            return services;
        }
    }
}